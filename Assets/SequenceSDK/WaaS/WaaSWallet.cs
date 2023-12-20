using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.Authentication;
using Sequence.Wallet;

namespace Sequence.WaaS
{
    public class WaaSWallet : IWallet
    {
        private HttpClient _httpClient;
        private Address _address;
        private string _sessionId;
        private EthWallet _sessionWallet;
        private DataKey _awsDataKey;
        private int _waasProjectId;
        private string _waasVersion;

        public WaaSWallet(Address address, string sessionId, EthWallet sessionWallet, DataKey awsDataKey, int waasProjectId, string waasVersion)
        {
            _address = address;
            _sessionId = sessionId;
            _sessionWallet = sessionWallet;
            _awsDataKey = awsDataKey;
            _waasProjectId = waasProjectId;
            _waasVersion = waasVersion;
            _httpClient = new HttpClient("https://d14tu8valot5m0.cloudfront.net/rpc/WaasAuthenticator/SendIntent");
        }

        public Task<CreatePartnerReturn> CreatePartner(CreatePartnerArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<CreatePartnerArgs, CreatePartnerReturn>("CreatePartner", args, headers);
        }

        public Task<UpdatePartnerReturn> UpdatePartner(UpdatePartnerArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<UpdatePartnerArgs, UpdatePartnerReturn>("UpdatePartner", args, headers);
        }

        public Task<CreatePartnerWalletConfigReturn> CreatePartnerWalletConfig(CreatePartnerWalletConfigArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<CreatePartnerWalletConfigArgs, CreatePartnerWalletConfigReturn>("CreatePartnerWalletConfig", args, headers);
        }

        public Task<PartnerWalletConfigReturn> PartnerWalletConfig(PartnerWalletConfigArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<PartnerWalletConfigArgs, PartnerWalletConfigReturn>("PartnerWalletConfig", args, headers);
        }

        public Task<DeployPartnerParentWalletReturn> DeployPartnerParentWallet(DeployPartnerParentWalletArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<DeployPartnerParentWalletArgs, DeployPartnerParentWalletReturn>("DeployPartnerParentWallet", args, headers);
        }

        public Task<AddPartnerWalletSignerReturn> AddPartnerWalletSigner(AddPartnerWalletSignerArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<AddPartnerWalletSignerArgs, AddPartnerWalletSignerReturn>("AddPartnerWalletSigner", args, headers);
        }

        public Task<RemovePartnerWalletSignerReturn> RemovePartnerWalletSigner(RemovePartnerWalletSignerArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<RemovePartnerWalletSignerArgs, RemovePartnerWalletSignerReturn>("RemovePartnerWalletSigner", args, headers);
        }

        public Task<ListPartnerWalletSignersReturn> ListPartnerWalletSigners(ListPartnerWalletSignersArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<ListPartnerWalletSignersArgs, ListPartnerWalletSignersReturn>("ListPartnerWalletSigners", args, headers);
        }

        public Task<PartnerWalletsReturn> PartnerWallets(PartnerWalletsArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<PartnerWalletsArgs, PartnerWalletsReturn>("PartnerWallets", args, headers);
        }

        public Task<CreateWalletReturn> CreateWallet(CreateWalletArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<CreateWalletArgs, CreateWalletReturn>("CreateWallet", args, headers);
        }

        public Task<GetWalletAddressReturn> GetWalletAddress(GetWalletAddressArgs args, Dictionary<string, string> headers = null)
        {
            GetWalletAddressReturn result = new GetWalletAddressReturn(this._address.Value);
            return Task.FromResult(result);
        }

        public Task<DeployWalletReturn> DeployWallet(DeployWalletArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<DeployWalletArgs, DeployWalletReturn>("DeployWallet", args, headers);
        }

        public Task<WalletsReturn> Wallets(WalletsArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<WalletsArgs, WalletsReturn>("Wallets", args, headers);
        }

        public Task<SignMessageReturn> SignMessage(SignMessageArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<SignMessageArgs, SignMessageReturn>("SignMessage", args, headers);
        }

        public Task<IsValidMessageSignatureReturn> IsValidMessageSignature(IsValidMessageSignatureArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<IsValidMessageSignatureArgs, IsValidMessageSignatureReturn>("IsValidMessageSignature", args, headers);
        }

        public Task<SendTransactionReturn> SendTransaction(SendTransactionArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<SendTransactionArgs, SendTransactionReturn>("SendTransaction", args, headers);
        }

        public Task<SendTransactionBatchReturn> SendTransactionBatch(SendTransactionBatchArgs args, Dictionary<string, string> headers = null)
        {
            return _httpClient.SendRequest<SendTransactionBatchArgs, SendTransactionBatchReturn>("SendTransactionBatch", args, headers);
        }
    }
}