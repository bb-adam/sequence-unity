namespace Sequence.EmbeddedWallet
{
    [System.Serializable]
    public class IsValidMessageSignatureReturn
    {
        public bool isValid;
        public IsValidMessageSignatureReturn(bool IsValid)
        {
             isValid =  IsValid;
        }
    }



}