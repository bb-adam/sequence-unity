using System;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.Provider;
using Sequence.Transactions;
using Sequence.Utils;
using Sequence.WaaS;
using Sequence.WaaS.Authentication;
using SequenceSDK.Ethereum.Utils;
using SequenceSDK.WaaS;
using TMPro;
using UnityEngine;
using IWallet = Sequence.Wallet.IWallet;
using StringExtensions = Sequence.Utils.StringExtensions;

namespace Sequence.Demo
{
    public class WaaSDemoPage : UIPage
    {
        [SerializeField] private TextMeshProUGUI _resultText;
        
        private WaaSWallet _wallet;
        private Address _address;
        private IWallet _adapter;
        
        public override void Open(params object[] args)
        {
            _wallet =
                args.GetObjectOfTypeIfExists<WaaSWallet>();
            if (_wallet == default)
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(WaaSWallet)} as an argument");
            }
            _gameObject.SetActive(true);
            _animator.AnimateIn( _openAnimationDurationInSeconds);

            _address = _wallet.GetWalletAddress();
            
            _wallet.OnSignMessageComplete += OnSignMessageComplete;
            _wallet.OnSendTransactionComplete += OnSuccessfulTransaction;
            _wallet.OnSendTransactionFailed += OnFailedTransaction;
            _wallet.OnDropSessionComplete += OnDropSessionComplete;

            CreateAdapter();
        }

        private async Task CreateAdapter()
        {
            _adapter = new WaaSToWalletAdapter(_wallet);
        }
        
        public void SignMessage()
        {
            _wallet.SignMessage(new SignMessageArgs(_address, Chain.Polygon, "Hello World!"));
        }
        
        private void OnSignMessageComplete(SignMessageReturn result)
        {
            _resultText.text = result.signature;
        }

        public void SendTransfer()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address,
                Chain.Polygon,
                new SequenceSDK.WaaS.Transaction[]
                {
                    new RawTransaction("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "1")
                }));
        }

        public void DeployERC20()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address,
                Chain.Polygon,
                new Transaction[]
                {
                    new DelayedEncode(_address, "0", new DelayedEncodeData(
                        "createContract(bytes)", new object[]
                        {
                            "0x60806040523480156200001157600080fd5b506040518060400160405280600981526020017f54657374546f6b656e00000000000000000000000000000000000000000000008152506040518060400160405280600381526020017f535454000000000000000000000000000000000000000000000000000000000081525081600390816200008f919062000412565b508060049081620000a1919062000412565b505050620000c4620000b8620000ca60201b60201c565b620000d260201b60201c565b620004f9565b600033905090565b6000600560009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905081600560006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055508173ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff167f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e060405160405180910390a35050565b600081519050919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b600060028204905060018216806200021a57607f821691505b60208210810362000230576200022f620001d2565b5b50919050565b60008190508160005260206000209050919050565b60006020601f8301049050919050565b600082821b905092915050565b6000600883026200029a7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff826200025b565b620002a686836200025b565b95508019841693508086168417925050509392505050565b6000819050919050565b6000819050919050565b6000620002f3620002ed620002e784620002be565b620002c8565b620002be565b9050919050565b6000819050919050565b6200030f83620002d2565b620003276200031e82620002fa565b84845462000268565b825550505050565b600090565b6200033e6200032f565b6200034b81848462000304565b505050565b5b8181101562000373576200036760008262000334565b60018101905062000351565b5050565b601f821115620003c2576200038c8162000236565b62000397846200024b565b81016020851015620003a7578190505b620003bf620003b6856200024b565b83018262000350565b50505b505050565b600082821c905092915050565b6000620003e760001984600802620003c7565b1980831691505092915050565b6000620004028383620003d4565b9150826002028217905092915050565b6200041d8262000198565b67ffffffffffffffff811115620004395762000438620001a3565b5b62000445825462000201565b6200045282828562000377565b600060209050601f8311600181146200048a576000841562000475578287015190505b620004818582620003f4565b865550620004f1565b601f1984166200049a8662000236565b60005b82811015620004c4578489015182556001820191506020850194506020810190506200049d565b86831015620004e45784890151620004e0601f891682620003d4565b8355505b6001600288020188555050505b505050505050565b611b8080620005096000396000f3fe608060405234801561001057600080fd5b506004361061010b5760003560e01c806370a08231116100a257806395d89b411161007157806395d89b41146102a6578063a457c2d7146102c4578063a9059cbb146102f4578063dd62ed3e14610324578063f2fde38b146103545761010b565b806370a0823114610232578063715018a61461026257806379cc67901461026c5780638da5cb5b146102885761010b565b8063313ce567116100de578063313ce567146101ac57806339509351146101ca57806340c10f19146101fa57806342966c68146102165761010b565b806306fdde0314610110578063095ea7b31461012e57806318160ddd1461015e57806323b872dd1461017c575b600080fd5b610118610370565b6040516101259190611178565b60405180910390f35b61014860048036038101906101439190611233565b610402565b604051610155919061128e565b60405180910390f35b610166610425565b60405161017391906112b8565b60405180910390f35b610196600480360381019061019191906112d3565b61042f565b6040516101a3919061128e565b60405180910390f35b6101b461045e565b6040516101c19190611342565b60405180910390f35b6101e460048036038101906101df9190611233565b610467565b6040516101f1919061128e565b60405180910390f35b610214600480360381019061020f9190611233565b61049e565b005b610230600480360381019061022b919061135d565b6104b4565b005b61024c6004803603810190610247919061138a565b6104c8565b60405161025991906112b8565b60405180910390f35b61026a610510565b005b61028660048036038101906102819190611233565b610524565b005b610290610544565b60405161029d91906113c6565b60405180910390f35b6102ae61056e565b6040516102bb9190611178565b60405180910390f35b6102de60048036038101906102d99190611233565b610600565b6040516102eb919061128e565b60405180910390f35b61030e60048036038101906103099190611233565b610677565b60405161031b919061128e565b60405180910390f35b61033e600480360381019061033991906113e1565b61069a565b60405161034b91906112b8565b60405180910390f35b61036e6004803603810190610369919061138a565b610721565b005b60606003805461037f90611450565b80601f01602080910402602001604051908101604052809291908181526020018280546103ab90611450565b80156103f85780601f106103cd576101008083540402835291602001916103f8565b820191906000526020600020905b8154815290600101906020018083116103db57829003601f168201915b5050505050905090565b60008061040d6107a4565b905061041a8185856107ac565b600191505092915050565b6000600254905090565b60008061043a6107a4565b9050610447858285610975565b610452858585610a01565b60019150509392505050565b60006012905090565b6000806104726107a4565b9050610493818585610484858961069a565b61048e91906114b0565b6107ac565b600191505092915050565b6104a6610c77565b6104b08282610cf5565b5050565b6104c56104bf6107a4565b82610e4b565b50565b60008060008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020549050919050565b610518610c77565b6105226000611018565b565b610536826105306107a4565b83610975565b6105408282610e4b565b5050565b6000600560009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905090565b60606004805461057d90611450565b80601f01602080910402602001604051908101604052809291908181526020018280546105a990611450565b80156105f65780601f106105cb576101008083540402835291602001916105f6565b820191906000526020600020905b8154815290600101906020018083116105d957829003601f168201915b5050505050905090565b60008061060b6107a4565b90506000610619828661069a565b90508381101561065e576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161065590611556565b60405180910390fd5b61066b82868684036107ac565b60019250505092915050565b6000806106826107a4565b905061068f818585610a01565b600191505092915050565b6000600160008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002054905092915050565b610729610c77565b600073ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff1603610798576040517f08c379a000000000000000000000000000000000000000000000000000000000815260040161078f906115e8565b60405180910390fd5b6107a181611018565b50565b600033905090565b600073ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff160361081b576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016108129061167a565b60405180910390fd5b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff160361088a576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016108819061170c565b60405180910390fd5b80600160008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819055508173ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff167f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b9258360405161096891906112b8565b60405180910390a3505050565b6000610981848461069a565b90507fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff81146109fb57818110156109ed576040517f08c379a00000000000000000000000000000000000000000000000000000000081526004016109e490611778565b60405180910390fd5b6109fa84848484036107ac565b5b50505050565b600073ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff1603610a70576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610a679061180a565b60405180910390fd5b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff1603610adf576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610ad69061189c565b60405180910390fd5b610aea8383836110de565b60008060008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002054905081811015610b70576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610b679061192e565b60405180910390fd5b8181036000808673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002081905550816000808573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600082825401925050819055508273ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef84604051610c5e91906112b8565b60405180910390a3610c718484846110e3565b50505050565b610c7f6107a4565b73ffffffffffffffffffffffffffffffffffffffff16610c9d610544565b73ffffffffffffffffffffffffffffffffffffffff1614610cf3576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610cea9061199a565b60405180910390fd5b565b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff1603610d64576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610d5b90611a06565b60405180910390fd5b610d70600083836110de565b8060026000828254610d8291906114b0565b92505081905550806000808473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600082825401925050819055508173ffffffffffffffffffffffffffffffffffffffff16600073ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef83604051610e3391906112b8565b60405180910390a3610e47600083836110e3565b5050565b600073ffffffffffffffffffffffffffffffffffffffff168273ffffffffffffffffffffffffffffffffffffffff1603610eba576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610eb190611a98565b60405180910390fd5b610ec6826000836110de565b60008060008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002054905081811015610f4c576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610f4390611b2a565b60405180910390fd5b8181036000808573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000208190555081600260008282540392505081905550600073ffffffffffffffffffffffffffffffffffffffff168373ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef84604051610fff91906112b8565b60405180910390a3611013836000846110e3565b505050565b6000600560009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16905081600560006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055508173ffffffffffffffffffffffffffffffffffffffff168173ffffffffffffffffffffffffffffffffffffffff167f8be0079c531659141344cd1fd0a4f28419497f9722a3daafe3b4186f6b6457e060405160405180910390a35050565b505050565b505050565b600081519050919050565b600082825260208201905092915050565b60005b83811015611122578082015181840152602081019050611107565b60008484015250505050565b6000601f19601f8301169050919050565b600061114a826110e8565b61115481856110f3565b9350611164818560208601611104565b61116d8161112e565b840191505092915050565b60006020820190508181036000830152611192818461113f565b905092915050565b600080fd5b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b60006111ca8261119f565b9050919050565b6111da816111bf565b81146111e557600080fd5b50565b6000813590506111f7816111d1565b92915050565b6000819050919050565b611210816111fd565b811461121b57600080fd5b50565b60008135905061122d81611207565b92915050565b6000806040838503121561124a5761124961119a565b5b6000611258858286016111e8565b92505060206112698582860161121e565b9150509250929050565b60008115159050919050565b61128881611273565b82525050565b60006020820190506112a3600083018461127f565b92915050565b6112b2816111fd565b82525050565b60006020820190506112cd60008301846112a9565b92915050565b6000806000606084860312156112ec576112eb61119a565b5b60006112fa868287016111e8565b935050602061130b868287016111e8565b925050604061131c8682870161121e565b9150509250925092565b600060ff82169050919050565b61133c81611326565b82525050565b60006020820190506113576000830184611333565b92915050565b6000602082840312156113735761137261119a565b5b60006113818482850161121e565b91505092915050565b6000602082840312156113a05761139f61119a565b5b60006113ae848285016111e8565b91505092915050565b6113c0816111bf565b82525050565b60006020820190506113db60008301846113b7565b92915050565b600080604083850312156113f8576113f761119a565b5b6000611406858286016111e8565b9250506020611417858286016111e8565b9150509250929050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b6000600282049050600182168061146857607f821691505b60208210810361147b5761147a611421565b5b50919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601160045260246000fd5b60006114bb826111fd565b91506114c6836111fd565b92508282019050808211156114de576114dd611481565b5b92915050565b7f45524332303a2064656372656173656420616c6c6f77616e63652062656c6f7760008201527f207a65726f000000000000000000000000000000000000000000000000000000602082015250565b60006115406025836110f3565b915061154b826114e4565b604082019050919050565b6000602082019050818103600083015261156f81611533565b9050919050565b7f4f776e61626c653a206e6577206f776e657220697320746865207a65726f206160008201527f6464726573730000000000000000000000000000000000000000000000000000602082015250565b60006115d26026836110f3565b91506115dd82611576565b604082019050919050565b60006020820190508181036000830152611601816115c5565b9050919050565b7f45524332303a20617070726f76652066726f6d20746865207a65726f2061646460008201527f7265737300000000000000000000000000000000000000000000000000000000602082015250565b60006116646024836110f3565b915061166f82611608565b604082019050919050565b6000602082019050818103600083015261169381611657565b9050919050565b7f45524332303a20617070726f766520746f20746865207a65726f20616464726560008201527f7373000000000000000000000000000000000000000000000000000000000000602082015250565b60006116f66022836110f3565b91506117018261169a565b604082019050919050565b60006020820190508181036000830152611725816116e9565b9050919050565b7f45524332303a20696e73756666696369656e7420616c6c6f77616e6365000000600082015250565b6000611762601d836110f3565b915061176d8261172c565b602082019050919050565b6000602082019050818103600083015261179181611755565b9050919050565b7f45524332303a207472616e736665722066726f6d20746865207a65726f20616460008201527f6472657373000000000000000000000000000000000000000000000000000000602082015250565b60006117f46025836110f3565b91506117ff82611798565b604082019050919050565b60006020820190508181036000830152611823816117e7565b9050919050565b7f45524332303a207472616e7366657220746f20746865207a65726f206164647260008201527f6573730000000000000000000000000000000000000000000000000000000000602082015250565b60006118866023836110f3565b91506118918261182a565b604082019050919050565b600060208201905081810360008301526118b581611879565b9050919050565b7f45524332303a207472616e7366657220616d6f756e742065786365656473206260008201527f616c616e63650000000000000000000000000000000000000000000000000000602082015250565b60006119186026836110f3565b9150611923826118bc565b604082019050919050565b600060208201905081810360008301526119478161190b565b9050919050565b7f4f776e61626c653a2063616c6c6572206973206e6f7420746865206f776e6572600082015250565b60006119846020836110f3565b915061198f8261194e565b602082019050919050565b600060208201905081810360008301526119b381611977565b9050919050565b7f45524332303a206d696e7420746f20746865207a65726f206164647265737300600082015250565b60006119f0601f836110f3565b91506119fb826119ba565b602082019050919050565b60006020820190508181036000830152611a1f816119e3565b9050919050565b7f45524332303a206275726e2066726f6d20746865207a65726f2061646472657360008201527f7300000000000000000000000000000000000000000000000000000000000000602082015250565b6000611a826021836110f3565b9150611a8d82611a26565b604082019050919050565b60006020820190508181036000830152611ab181611a75565b9050919050565b7f45524332303a206275726e20616d6f756e7420657863656564732062616c616e60008201527f6365000000000000000000000000000000000000000000000000000000000000602082015250565b6000611b146022836110f3565b9150611b1f82611ab8565b604082019050919050565b60006020820190508181036000830152611b4381611b07565b905091905056fea26469706673582212204f5863a5c182fce47d84abd8a2c644c72bf23a527680e14ec57349d38f1cd1a564736f6c63430008110033",
                        }, "createContract"))
                }));
        }
        
        private void OnSuccessfulTransaction(SuccessfulTransactionReturn result)
        {
            _resultText.text = $"https://polygonscan.com/tx/{result.txHash}";
            Debug.Log("Transaction successful: " + result.txHash);
        }
        
        private void OnFailedTransaction(FailedTransactionReturn result)
        {
            _resultText.text = result.error;
            Debug.LogError("Transaction failed: " + result.error);
        }

        public void SendFailingTransfer()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address,
                Chain.Polygon,
                new SequenceSDK.WaaS.Transaction[]
                {
                    new RawTransaction("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "99000000000000000000")
                }));
        }

        public void DropSession()
        {
            _wallet.DropThisSession();
        }
        
        private void OnDropSessionComplete(string droppedSessionId)
        {
            Debug.Log("Session dropped: " + droppedSessionId);
            _wallet = null;
            Close();
        }

        public void SendErc20Transfer()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new SendERC20(
                        "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "1"),
                }));
        }
        
        public void SendErc721Transfer()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new SendERC721(
                        "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "54530968763798660137294927684252503703134533114052628080002308208148824588621"),
                }));
        }
        
        public void SendErc1155Transfer()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new SendERC1155(
                        "0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        new SendERC1155Values[]
                        {
                            new SendERC1155Values("86", "1")
                        }),
                }));
        }

        public void SendMultipleTransferTypes()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new RawTransaction("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", DecimalNormalizer.Normalize(1)),
                    new SendERC20(
                        "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "1"),
                    new SendERC721(
                        "0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        "54530968763798660137294927684252503703134533114052628080002308208148824588621"),
                    new SendERC1155(
                        "0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb",
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        new SendERC1155Values[]
                        {
                            new SendERC1155Values("86", "1")
                        }),
                    new DelayedEncode("0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359", "0", new DelayedEncodeData(
                        "transfer(address,uint256)",
                        new object[]
                        {
                            "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "1"
                        },
                        "transfer")),
                }));
        }

        public void SendWithAdapter()
        {
            DoSendWithAdapter();
        }

        private async Task DoSendWithAdapter()
        {
            ERC721 nft = new ERC721("0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f");
            var receipt = await nft.TransferFrom(_adapter.GetAddress(), "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", 
                    "54530968763798660137294927684252503703134533114052628080002308208148824588621")
                .SendTransactionMethodAndWaitForReceipt(_adapter, new SequenceEthClient("https://polygon-bor.publicnode.com"));
            Debug.LogError($"Transaction hash: {receipt.transactionHash}");
            
        }

        public void SendMultipleWithAdapter()
        {
            DoSendMultipleWithAdapter();
        }

        private async Task DoSendMultipleWithAdapter()
        {
            ERC721 nft = new ERC721("0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f");
            ERC1155 sft = new ERC1155("0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb");
            SequenceEthClient client = new SequenceEthClient("https://polygon-bor.publicnode.com");
            var nftTransfer = await nft.TransferFrom(_adapter.GetAddress(),
                "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                "54530968763798660137294927684252503703134533114052628080002308208148824588621")(client, new ContractCall(_adapter.GetAddress()));
            var sftTransfer = await sft.SafeTransferFrom(_adapter.GetAddress(),
                "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                "86",
                1)(client, new ContractCall(_adapter.GetAddress()));

            var receipt = await _adapter.SendTransactionBatchAndWaitForReceipts(client, new EthTransaction[]
            {
                nftTransfer, sftTransfer
            });
            Debug.LogError($"Transaction hash: {receipt[0].transactionHash}");
            
            // or
            
            // _wallet.SendTransaction(new SendTransactionArgs(
            //     _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
            //     {
            //         new RawTransaction(nftTransfer),
            //         new RawTransaction(sftTransfer),
            //     }));
        }
        
        public void SendMultipleWithAdapter2()
        {
            DoSendMultipleWithAdapter2();
        }

        private async Task DoSendMultipleWithAdapter2()
        {
            ERC721 nft = new ERC721("0xa9a6A3626993D487d2Dbda3173cf58cA1a9D9e9f");
            ERC1155 sft = new ERC1155("0x44b3f42e2bf34f62868ff9e9dab7c2f807ba97cb");
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new RawTransaction(nft.Contract, "transferFrom", _adapter.GetAddress().Value,
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        BigInteger.Parse("54530968763798660137294927684252503703134533114052628080002308208148824588621")),
                    new RawTransaction(sft.Contract, "safeTransferFrom", _adapter.GetAddress().Value,
                        "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",
                        86,
                        1, "data".ToByteArray()), // Todo figure out why data is required
                }));
        }

        public void DelayedEncode()
        {
            _wallet.SendTransaction(new SendTransactionArgs(
                _address, Chain.Polygon, new SequenceSDK.WaaS.Transaction[]
                {
                    new DelayedEncode("0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359", "0", new DelayedEncodeData(
                        "transfer(address,uint256)",
                        new object[]
                        {
                            "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f", "1"
                        },
                        "transfer")),
                }));
        }

        public void ListSessions()
        {
            _wallet.OnSessionsFound += OnSessionsListed;
            _wallet.ListSessions();
        }
        
        private void OnSessionsListed(WaaSSession[] sessions)
        {
            _resultText.text = $"Found {sessions.Length} sessions";
        }
    }
}