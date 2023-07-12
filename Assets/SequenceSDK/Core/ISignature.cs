using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Sequence.Core.Wallet;
using Sequence.Core.Provider;

namespace Sequence.Core
{
    public interface ISignature
    {
        // Threshold is the minimum signing weight required for a signature to be valid.
        int Threshold();

        // Checkpoint is the nonce of the wallet configuration that the signature applies to.
        BigInteger Checkpoint();

        // Recover derives the wallet configuration that the signature applies to.
        // Also returns the signature's weight.
        // If chainID is not provided, provider must be provided.
        // If provider is not provided, EIP-1271 signatures are assumed to be valid.
        // If signerSignatures is provided, it will be populated with the valid signer signatures of this signature.
        (Config, BigInteger) Recover(WalletContext context, 
                                            Digest digest, 
                                            Address wallet,
                                            BigInteger chainId,
                                            RPCProvider provider,
                                            List<SignerSignatures> signerSignatures);

        // Recover a signature but only using the subdigest
        (Config, BigInteger) RecoverSubdigest(WalletContext context, 
                                                    Subdigest subdigest, 
                                                    RPCProvider provider,
                                                    List<SignerSignatures> signerSignatures);

        // Reduce returns an equivalent optimized signature.
        ISignature Reduce(Subdigest subdigest);

        // Join joins two signatures into one.
        ISignature Join(Subdigest subdigest, ISignature otherSignature);

        // Data is the raw signature data.
        byte[] Data();
    }
}