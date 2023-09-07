using Sequence.Contracts;
using UnityEngine;

namespace Sequence.Demo
{
    public class MockCurrencyConverter : ICurrencyConverter
    {
        public CurrencyValue ConvertToCurrency(float amount, ERC20 token)
        {
            return new CurrencyValue("$", amount + Random.Range(-amount * .25f, amount * .25f));
        }
    }
}