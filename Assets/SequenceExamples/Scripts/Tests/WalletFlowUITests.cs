using System;
using System.Collections;
using Sequence;
using Sequence.Demo;
using SequenceExamples.Scripts.Tests.Utils;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SequenceExamples.Scripts.Tests
{
    public class WalletFlowUITests : MonoBehaviour
    {
        private MonoBehaviour _testMonobehaviour;
            
        private SequenceUI _ui;
        private WalletPanel _walletPanel;
        private WalletPage _walletPage;
        private LoginPanel _loginPanel;
        private TransitionPanel _transitionPanel;

        private int _randomNumberOfTokensToFetch;
        private int _randomNumberOfNftsToFetch;

        public void Setup(MonoBehaviour testMonobehaviour, SequenceUI ui, WalletPanel walletPanel, WalletPage walletPage, LoginPanel loginPanel, TransitionPanel transitionPanel)
        {
            _testMonobehaviour = testMonobehaviour;
            _ui = ui;
            _walletPanel = walletPanel;
            _walletPage = walletPage;
            _loginPanel = loginPanel;
            _transitionPanel = transitionPanel;
        }

        public IEnumerator EndToEndTest()
        {
            _randomNumberOfTokensToFetch = Random.Range(0, 100);
            _randomNumberOfNftsToFetch = Random.Range(0, 1000);
            AssertWeAreOnTransitionPanel();
            yield return _testMonobehaviour.StartCoroutine(TransitionToWalletPageTest());
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
            yield return _testMonobehaviour.StartCoroutine(CloseWalletPanelTest());
            AssertWeAreOnTransitionPanel();
            _randomNumberOfTokensToFetch = Random.Range(0, 10);
            _randomNumberOfNftsToFetch = Random.Range(0, 100);
            yield return _testMonobehaviour.StartCoroutine(TransitionToWalletPageTest());
            yield return _testMonobehaviour.StartCoroutine(AssertWalletPageIsAsExpected());
            yield return _testMonobehaviour.StartCoroutine(TestTokenInfoPages());
        }

        private void AssertWeAreOnTransitionPanel()
        {
            Assert.IsFalse(_walletPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_walletPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsTrue(_transitionPanel.gameObject.activeInHierarchy);
        }

        private IEnumerator TransitionToWalletPageTest()
        {
            GameObject openWalletButtonGameObject = GameObject.Find("OpenWalletButton");
            Assert.IsNotNull(openWalletButtonGameObject);
            Button openWalletButton = openWalletButtonGameObject.GetComponent<Button>();
            Assert.IsNotNull(openWalletButton);

            _transitionPanel.TokenFetcher = new MockTokenContentFetcher(_randomNumberOfTokensToFetch, 0);
            _transitionPanel.NftFetcher = new MockNftContentFetcher(_randomNumberOfNftsToFetch, 0);
            Debug.Log($"Will fetch {_randomNumberOfTokensToFetch} tokens and {_randomNumberOfNftsToFetch} NFTs");
            
            openWalletButton.onClick.Invoke();
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertWeAreOnWalletPage();
        }

        private void AssertWeAreOnWalletPage()
        {
            Assert.IsTrue(_walletPanel.gameObject.activeInHierarchy);
            Assert.IsTrue(_walletPage.gameObject.activeInHierarchy);
            Assert.IsFalse(_loginPanel.gameObject.activeInHierarchy);
            Assert.IsFalse(_transitionPanel.gameObject.activeInHierarchy);
        }

        private IEnumerator AssertWalletPageIsAsExpected()
        {
            AssertPanelAssumptions_WalletPage();
            AssertWeAreOnWalletPage();
            yield return _testMonobehaviour.StartCoroutine(AssertWeLoadEnoughContent());
            AssertTokensAreAboveNFTs();
            AssertWeHaveAppropriateNetworkIcons();
            AssertBrandingIsBelowContent();
            yield return _testMonobehaviour.StartCoroutine(AssertValueChangeDisplayedCorrectly());
        }

        private void AssertPanelAssumptions_WalletPage()
        {
            Transform searchButtonTransform = _walletPanel.transform.FindAmongDecendants("SearchButton");
            Assert.IsTrue(searchButtonTransform.gameObject.activeInHierarchy);
            Transform backButtonTransform = _walletPanel.transform.FindAmongDecendants("BackButton");
            Assert.IsFalse(backButtonTransform.gameObject.activeInHierarchy);
        }

        private IEnumerator AssertWeLoadEnoughContent()
        {
            if (_transitionPanel.TokenFetcher is MockTokenContentFetcher mockTokenFetcher)
            {
                yield return new WaitForSeconds(_randomNumberOfTokensToFetch * (float)mockTokenFetcher.DelayInMilliseconds / 1000);
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.TokenFetcher)} type. Expected {typeof(MockTokenContentFetcher)}");
            }
            
            if (_transitionPanel.NftFetcher is MockNftContentFetcher mockNftFetcher)
            {
                yield return new WaitForSeconds(_randomNumberOfNftsToFetch * (float)mockNftFetcher.DelayInMilliseconds / 1000);
            }
            else
            {
                NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.NftFetcher)} type. Expected {typeof(MockNftContentFetcher)}");
            }
            
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;
            Debug.Log($"Fetched {_walletPage.CountFungibleTokensDisplayed()} tokens and a total of {contentLoaded} content");
            Debug.Log($"Expected to fetch {_randomNumberOfTokensToFetch} tokens, {_randomNumberOfNftsToFetch} NFTs, and {_randomNumberOfTokensToFetch + _randomNumberOfNftsToFetch} total content");
            Assert.AreEqual(_randomNumberOfTokensToFetch + _randomNumberOfNftsToFetch, contentLoaded);
            Assert.AreEqual(_randomNumberOfTokensToFetch, _walletPage.CountFungibleTokensDisplayed());
        }

        private void AssertTokensAreAboveNFTs()
        {
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;
            bool hasSeenNFT = false;
            bool finishedSeeingTokens = false;
            bool hasSeenToken = false;

            for (int i = 0; i < contentLoaded; i++)
            {
                Transform child = grid.transform.GetChild(i);
                TokenUIElement token = child.GetComponent<TokenUIElement>();
                NFTUIElement nft = child.GetComponent<NFTUIElement>();
                if (nft != null && token != null)
                {
                    throw new AssertionException("Encountered an element that is both a token and an NFT",
                        "A UI element should be one or the other, not both");
                }

                if (token != null)
                {
                    hasSeenToken = true;
                    if (hasSeenNFT)
                    {
                        throw new AssertionException(
                            "Encountered a token after already encountering an NFT", "NFTs should only be found after all tokens have been found");
                    }
                }
                else if (hasSeenToken)
                {
                    finishedSeeingTokens = true;
                }
                
                if (nft != null)
                {
                    hasSeenNFT = true;
                    if (hasSeenToken && !finishedSeeingTokens)
                    {
                        throw new AssertionException("Encountered an NFT before finished seeing tokens",
                            "We should only ever see NFTs before tokens if there are no tokens to see");
                    }
                }
            }
        }

        private void AssertWeHaveAppropriateNetworkIcons()
        {
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            TokenUIElement[] tokenUIElements = grid.GetComponentsInChildren<TokenUIElement>();
            int elements = tokenUIElements.Length;
            for (int i = 0; i < elements; i++)
            {
                Transform tokenInfoGroup = tokenUIElements[i].transform.Find("TokenInfoGroup");
                Assert.IsNotNull(tokenInfoGroup);
                Image networkIconImage = tokenInfoGroup.GetComponentInChildren<Image>();
                Assert.IsNotNull(networkIconImage);
                Assert.AreEqual(tokenUIElements[i].NetworkIcons.GetIcon(tokenUIElements[i].GetNetwork()), networkIconImage.sprite);
            }
        }

        private void AssertBrandingIsBelowContent()
        {
            GameObject branding = GameObject.Find("PoweredBySequenceText");
            Assert.IsNotNull(branding);
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);

            RectTransform bottomContent =
                grid.transform.GetChild(_randomNumberOfTokensToFetch + _randomNumberOfNftsToFetch - 1).GetComponent<RectTransform>();
            RectTransform brandingTransform = branding.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            bottomContent.GetWorldCorners(corners);
            float bottomOfContentYPosition = corners[0].y;
            brandingTransform.GetWorldCorners(corners);
            float topOfBrandingYPosition = corners[1].y;
            Assert.IsTrue(topOfBrandingYPosition < bottomOfContentYPosition);
        }

        private IEnumerator AssertValueChangeDisplayedCorrectly()
        {
            TokenUIElement token = FindObjectOfType<TokenUIElement>();
            Assert.IsNotNull(token);
            Transform percentChange = token.transform.Find("PercentChangeText");
            Assert.IsNotNull(percentChange);
            TextMeshProUGUI percentChangeText = percentChange.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(percentChangeText);
            Transform balance = token.transform.Find("BalanceText");
            Assert.IsNotNull(balance);
            TextMeshProUGUI balanceText = balance.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(balanceText);

            yield return new WaitForSecondsRealtime(_walletPage.TimeBetweenTokenValueRefreshesInSeconds);
            
            AssertAppropriateColorPercentChangeText(percentChangeText);
            
            token.RefreshWithBalance(5);
            
            AssertAppropriateColorPercentChangeText(percentChangeText);
            Assert.AreEqual("0.00%", percentChangeText.text);
            
            balanceText.text.AssertStartsWith("5 ");

            yield return new WaitForSecondsRealtime(_walletPage.TimeBetweenTokenValueRefreshesInSeconds);
            
            AssertAppropriateColorPercentChangeText(percentChangeText);
            Assert.AreNotEqual("0.00%", percentChangeText.text);
        }

        private void AssertAppropriateColorPercentChangeText(TextMeshProUGUI text)
        {
            if (text.text[0] == '+')
            {
                Assert.AreEqual(Color.green, text.color);
            }else if (text.text[0] == '-')
            {
                Assert.AreEqual(Color.red, text.color);
            }
            else
            {
                Assert.AreNotEqual(Color.green, text.color);
                Assert.AreNotEqual(Color.red, text.color);
            }
        }

        private IEnumerator CloseWalletPanelTest()
        {
            GameObject closeWallet = GameObject.Find("CloseWalletButton");
            Assert.IsNotNull(closeWallet);
            Button closeWalletButton = closeWallet.GetComponent<Button>();
            Assert.IsNotNull(closeWalletButton);
            
            closeWalletButton.onClick.Invoke();
            yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
            
            AssertWeAreOnTransitionPanel();
        }

        private IEnumerator TestTokenInfoPages()
        {
            GameObject grid = GameObject.Find("Grid");
            Assert.IsNotNull(grid);
            int contentLoaded = grid.transform.childCount;

            for (int i = 0; i < contentLoaded; i++)
            {
                if (i >= 5)
                {
                    // Finish after testing the first five to save time - if it works for the first few, it should work for the remainder
                    break;
                }
                
                Transform child = grid.transform.GetChild(i);
                TokenUIElement token = child.GetComponent<TokenUIElement>();
                if (token == null)
                {
                    // Test is complete - we have gone through all the TokenUIElements (they should be displayed first before NFTs)
                    break;
                }
                
                int randomNumberOfTransactionsToFetch = Random.Range(0, 30);
                token.TransactionDetailsFetcher = new MockTransactionDetailsFetcher(randomNumberOfTransactionsToFetch, 0);
                MockTransactionDetailsFetcher fetcher = (MockTransactionDetailsFetcher)token.TransactionDetailsFetcher;
                Button button = token.GetComponent<Button>();
                Assert.IsNotNull(button);
                
                button.onClick.Invoke();
                yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
                
                yield return _testMonobehaviour.StartCoroutine(AssertTokenInfoPageIsAsExpected(token.GetNetwork(), randomNumberOfTransactionsToFetch, fetcher.DelayInMilliseconds));
                
                AssertPanelAssumptions_InfoPage();
                Transform backButtonTransform = _walletPanel.transform.FindAmongDecendants("BackButton");
                Assert.IsNotNull(backButtonTransform);
                Button backButton = backButtonTransform.GetComponent<Button>();
                Assert.IsNotNull(backButton);
                backButton.onClick.Invoke();
                
                yield return new WaitForSeconds(UITestHarness.WaitForAnimationTime); // Wait for next page to animate in
                
                AssertPanelAssumptions_WalletPage();
                
                // Wait for tokens to load again
                if (_transitionPanel.TokenFetcher is MockTokenContentFetcher mockTokenFetcher)
                {
                    yield return new WaitForSeconds(_randomNumberOfTokensToFetch * (float)mockTokenFetcher.DelayInMilliseconds / 1000);
                }
                else
                {
                    NUnit.Framework.Assert.Fail($"Unexpected {nameof(_transitionPanel.TokenFetcher)} type. Expected {typeof(MockTokenContentFetcher)}");
                }
            }

            yield return null;
        }

        private void AssertPanelAssumptions_InfoPage()
        {
            Transform searchButtonTransform = _walletPanel.transform.FindAmongDecendants("SearchButton");
            Assert.IsFalse(searchButtonTransform.gameObject.activeInHierarchy);
            Transform backButtonTransform = _walletPanel.transform.FindAmongDecendants("BackButton");
            Assert.IsTrue(backButtonTransform.gameObject.activeInHierarchy);
        }

        private IEnumerator AssertTokenInfoPageIsAsExpected(Chain network, int randomNumberOfTransactionsToFetch, int delayInMillisecondsBetweenFetches)
        {
            TokenInfoPage tokenInfo = FindObjectOfType<TokenInfoPage>();
            Assert.IsNotNull(tokenInfo);

            Transform networkBanner = tokenInfo.transform.Find("NetworkBanner");
            Assert.IsNotNull(networkBanner);
            Transform networkIcon = networkBanner.transform.Find("NetworkIcon");
            Assert.IsNotNull(networkIcon);
            Image networkIconImage = networkIcon.GetComponent<Image>();
            Assert.IsNotNull(networkIconImage);
            Assert.AreEqual(tokenInfo.GetNetworkIcon(network), networkIconImage.sprite);
            Transform networkName = networkBanner.transform.Find("NetworkName");
            Assert.IsNotNull(networkName);
            TextMeshProUGUI networkNameText = networkName.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(networkNameText);
            Assert.AreEqual(ChainNames.NameOf[network], networkNameText.text);

            Transform currencyValue = tokenInfo.transform.Find("CurrencyValueText");
            Assert.IsNotNull(currencyValue);
            TextMeshProUGUI currencyValueText = currencyValue.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(currencyValueText);
            string currentCurrencyValue = currencyValueText.text;
            yield return new WaitForSecondsRealtime(tokenInfo.TimeBetweenTokenValueRefreshesInSeconds);
            Assert.AreNotEqual(currentCurrencyValue, currencyValueText.text);

            TransactionDetailsBlocksUITests transactionDetailsBlocksUITests =
                new TransactionDetailsBlocksUITests(_testMonobehaviour);
            yield return _testMonobehaviour.StartCoroutine(transactionDetailsBlocksUITests.AssertTransactionDetailsBlocksAreAsExpected(randomNumberOfTransactionsToFetch, delayInMillisecondsBetweenFetches));
        }
    }
}