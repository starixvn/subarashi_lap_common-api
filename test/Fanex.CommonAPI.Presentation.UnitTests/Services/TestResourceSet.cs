using System.Collections;
using System.Resources;

namespace Fanex.CommonAPI.Presentation.UnitTests.Services;

public class TestResourceSet : ResourceSet
{
    private readonly Dictionary<string, string> _resources;

    public TestResourceSet(Dictionary<string, string> resources)
    {
        _resources = resources;
    }

    public override IDictionaryEnumerator GetEnumerator()
    {
        return new TestDictionaryEnumerator(_resources);
    }

    private class TestDictionaryEnumerator : IDictionaryEnumerator
    {
        private readonly Dictionary<string, string> _resources;
        private readonly IEnumerator<KeyValuePair<string, string>> _enumerator;

        public TestDictionaryEnumerator(Dictionary<string, string> resources)
        {
            _resources = resources;
            _enumerator = resources.GetEnumerator();
        }

        public DictionaryEntry Entry => new(_enumerator.Current.Key, _enumerator.Current.Value);

        public object Key => _enumerator.Current.Key;

        public object Value => _enumerator.Current.Value;

        public object Current => Entry;

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();
    }
}