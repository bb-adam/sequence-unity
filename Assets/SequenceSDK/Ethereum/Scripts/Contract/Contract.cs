using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Provider;
using Sequence.Extensions;
using Sequence.Wallet;
using System.Text;

namespace Sequence.Contracts
{
    public class Contract
    {
        string address;
        public delegate Task<EthTransaction> CallContractFunctionTransactionCreator(IEthClient client, ContractCall contractCallInfo);


        public Contract(string contractAddress)
        {
            address = contractAddress;
        }

        public async Task<string> Deploy(string bytecode, params object[] constructorArgs)
        {
            throw new NotImplementedException();
        }

        public CallContractFunctionTransactionCreator CallFunction(string functionSignature, params object[] functionArgs)
        {
            string callData = ABI.ABI.Pack(functionSignature, functionArgs);
            return async (IEthClient client, ContractCall contractCallInfo) =>
            {
                TransactionCall call = new TransactionCall
                {
                    to = this.address,
                    value = contractCallInfo.value,
                    data = callData,
                };
                BigInteger gasLimitEstimate = await client.EstimateGas(call);

                if (contractCallInfo.gasPrice == 0)
                {
                    contractCallInfo.gasPrice = await client.SuggestGasPrice();
                }
                call.gasPrice = contractCallInfo.gasPrice;

                EthTransaction transaction = new EthTransaction(
                    contractCallInfo.nonce,
                    call.gasPrice,
                    gasLimitEstimate,
                    call.to,
                    call.value,
                    callData);

                return transaction;
            };
        }

        public async Task<string> QueryContract(IEthClient client, string functionSignature, params object[] args)
        {
            string data = ABI.ABI.Pack(functionSignature, args);
            string to = address;
            object[] toSendParams = new object[] {
                new
                {
                    to,
                    data
                }
            };
            return await client.CallContract(this.address, toSendParams);
        }

        public async Task<T> GetEventLog<T>(string eventName, BigInteger blockNumber)
        {
            throw new NotImplementedException();
        }        
    }

    public class ContractCall
    {
        public BigInteger nonce;
        public BigInteger value;
        public BigInteger gasPrice;

        public ContractCall(BigInteger nonce, BigInteger? value = null, BigInteger? gasPrice = null)
        {
            if (value == null)
            {
                value = BigInteger.Zero;
            }
            if (gasPrice == null)
            {
                gasPrice = BigInteger.Zero;
            }

            if (nonce < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nonce));
            }
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (gasPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasPrice));
            }

            this.value = (BigInteger)value;
        }
    }
}


