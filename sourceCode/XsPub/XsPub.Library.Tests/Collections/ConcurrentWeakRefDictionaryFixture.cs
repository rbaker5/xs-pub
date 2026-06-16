using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using XsPub.Library.Utility;

namespace XsPub.Library.Tests.Collections
{
    [TestFixture(Ignore = true)]
    [RequiresThread]
    public class ConcurrentWeakRefDictionaryFixture
    {
        private ConcurrentBag<ConcurrentResult> MismatchResults = new ConcurrentBag<ConcurrentResult>();

        private class ConcurrentResult
        {
            public bool AddSuccess { get; set; }
            public bool ContainedKey { get; set; }
            public bool GetSuccess { get; set; }
            public bool GetMatched { get; set; }
            public bool RemoveSuccess { get; set; }
            public bool RemoveMatched { get; set; }
            public bool ObjMatchesGetOrAdd { get; set; }

            public override string ToString()
            {
                return
                    string.Format(
                        "Add: {0}, ContainedKey: {1}, Get: [{2}{3}], Remove: [{4}{5}], MatchesGetOrAdd: {6}",
                        SuccessFail(AddSuccess), ContainedKey, 
                        SuccessFail(GetSuccess), MatchNoMatch(GetMatched), 
                        SuccessFail(RemoveSuccess), MatchNoMatch(RemoveMatched), 
                        ObjMatchesGetOrAdd);
            }

            private string SuccessFail(bool success)
            {
                return success ? "Success" : "Fail";
            }

            private string MatchNoMatch(bool matched)
            {
                return matched ? string.Empty : ", NoMatch";
            }
        }

        private class Holder
        {
            public Holder(string id)
            {
                Obj = new Obj {Id = id};
                Id = id;
            }

            public void Clear()
            {
                Obj = null;
            }

            public static implicit operator string(Holder holder)
            {
                return holder.Id;
            }

            public static implicit operator Obj(Holder holder)
            {
                return holder.Obj;
            }

            public Obj Obj { get; set; }
            public string Id { get; set; }
        }

        private class Obj
        {
            public string Id { get; set; }
        }

        [Test]
        public void TestAddsNoGC()
        {
            var dictionary = new ConcurrentWeakRefDictionary<string, Obj>();
            var holder = new Holder("Test1");
            testAddNoGC(dictionary, holder, false);
        }

        private void testAddNoGC(ConcurrentWeakRefDictionary<string, Obj> dictionary, Holder holder, bool ignoreCount)
        {
            Assert.IsTrue(dictionary.TryAdd(holder, holder));
            Assert.IsTrue(dictionary.ContainsKey(holder));
            if (!ignoreCount) Assert.AreEqual(1, dictionary.Count);
            AssertGet(true, dictionary, holder);
            AssertRemove(true, dictionary, holder);
            Assert.AreEqual(holder.Obj, dictionary.GetOrAdd(holder, holder));
            if (!ignoreCount) Assert.AreEqual(1, dictionary.Count);
        }

        [Test]
        public void TestAddsForceGC()
        {
            var dictionary = new ConcurrentWeakRefDictionary<string, Obj>();
            var holder = new Holder("Test1");
            testAddWithGC(dictionary, holder, false);
        }

        private void testAddWithGC(ConcurrentWeakRefDictionary<string, Obj> dictionary, Holder holder, bool ignoreCount)
        {
            Assert.IsTrue(dictionary.TryAdd(holder, holder));
            holder.Clear();
            GC.Collect();
            GC.WaitForFullGCComplete();

            Assert.IsFalse(dictionary.ContainsKey(holder.Id));
            if (!ignoreCount) Assert.AreEqual(0, dictionary.Count);
            
            AssertGet(false, dictionary, holder);
            AssertRemove(false, dictionary, holder);

            Assert.AreEqual(null, dictionary.GetOrAdd(holder, holder));
            if (!ignoreCount) Assert.AreEqual(1, dictionary.Count);
            AssertRemove(true, dictionary, holder);
        }

        private void testAddGetAfterGC(ConcurrentWeakRefDictionary<string, Obj> dictionary, Holder holder, bool ignoreCount)
        {
            Assert.IsTrue(dictionary.TryAdd(holder, holder));
            holder.Clear();
            GC.Collect();
            GC.WaitForFullGCComplete();

            AssertGet(false, dictionary, holder);
            Assert.IsFalse(dictionary.ContainsKey(holder.Id));
            if (!ignoreCount) Assert.AreEqual(0, dictionary.Count);
            
            AssertRemove(false, dictionary, holder);

            Assert.AreEqual(null, dictionary.GetOrAdd(holder, holder));
            if (!ignoreCount) Assert.AreEqual(1, dictionary.Count);
            AssertRemove(true, dictionary, holder);
        }

        [Test]
        public void TestConcurrentAddsNoGC()
        {
            var range = Enumerable.Range(0, 1000);
            var holders = range.Select(index => new Holder("Holder" + index));
            var dictionary = new ConcurrentWeakRefDictionary<string, Obj>();

            holders.AsParallel().ForAll(holder => testAddNoGC(dictionary, holder, true));
        }

        [Test]
        public void TestConcurrentAddsWithGC()
        {
            var range = Enumerable.Range(0, 1000);
            var holders = range.Select(index => new Holder("Holder" + index));
            var dictionary = new ConcurrentWeakRefDictionary<string, Obj>();

            holders.AsParallel().ForAll(holder => testAddWithGC(dictionary, holder, true));
        }

        [Test]
        public void TestConcurrentAddGetAfterGC()
        {
            var range = Enumerable.Range(0, 1000);
            var holders = range.Select(index => new Holder("Holder" + index));
            var dictionary = new ConcurrentWeakRefDictionary<string, Obj>();

            holders.AsParallel().ForAll(holder => testAddGetAfterGC(dictionary, holder, true));
        }

        [Test]
        public void TestFullyConcurrentAddNoGC()
        {
            var range = Enumerable.Range(0, 100);
            var range2 = Enumerable.Range(0, 50);
            var holders = range.Select(index => new Holder("Holder" + index));
            var dictionary = new ConcurrentWeakRefDictionary<string, Obj>();

            var tasks = range2.AsParallel().Select(index => System.Threading.Tasks.Task.Factory.StartNew(() => holders.AsParallel().ForAll(holder => testFullyConcurrentAddNoGC(dictionary, holder))));
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

            Console.WriteLine("AddSuccess Count: {0}", MismatchResults.Count(result => result.AddSuccess));
            Console.WriteLine("ContainedKey Count: {0}", MismatchResults.Count(result => result.ContainedKey));
            Console.WriteLine("GetSuccess Count: {0}", MismatchResults.Count(result => result.GetSuccess));
            Console.WriteLine("GetMatched Count: {0}", MismatchResults.Count(result => result.GetMatched));
            Console.WriteLine("RemoveSuccess Count: {0}", MismatchResults.Count(result => result.RemoveSuccess));
            Console.WriteLine("RemoveMatched Count: {0}", MismatchResults.Count(result => result.RemoveMatched));
            Console.WriteLine("MatchesGetOrAdd Count: {0}", MismatchResults.Count(result => result.ObjMatchesGetOrAdd));
        }

        [Test]
        public void TestFullyConcurrentAddWithClear()
        {
            var range = Enumerable.Range(0, 100);
            var range2 = Enumerable.Range(0, 50);
            var holders = range.Select(index => new Holder("Holder" + index));
            var dictionary = new ConcurrentWeakRefDictionary<string, Obj>();

            var tasks = range2.AsParallel().Select(index => System.Threading.Tasks.Task.Factory.StartNew(() => holders.AsParallel().ForAll(holder => testFullyConcurrentAddWithClear(dictionary, holder))));
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

            Console.WriteLine("AddSuccess Count: {0}", MismatchResults.Count(result => result.AddSuccess));
            Console.WriteLine("ContainedKey Count: {0}", MismatchResults.Count(result => result.ContainedKey));
            Console.WriteLine("GetSuccess Count: {0}", MismatchResults.Count(result => result.GetSuccess));
            Console.WriteLine("GetMatched Count: {0}", MismatchResults.Count(result => result.GetMatched));
            Console.WriteLine("RemoveSuccess Count: {0}", MismatchResults.Count(result => result.RemoveSuccess));
            Console.WriteLine("RemoveMatched Count: {0}", MismatchResults.Count(result => result.RemoveMatched));
            Console.WriteLine("MatchesGetOrAdd Count: {0}", MismatchResults.Count(result => result.ObjMatchesGetOrAdd));
        }

        [Test]
        public void TestFullyConcurrentAddWithGC()
        {
            var range = Enumerable.Range(0, 100);
            var range2 = Enumerable.Range(0, 50);
            var holders = range.Select(index => new Holder("Holder" + index));
            var dictionary = new ConcurrentWeakRefDictionary<string, Obj>();

            var tasks = range2.AsParallel().Select(index => System.Threading.Tasks.Task.Factory.StartNew(() => holders.AsParallel().ForAll(holder => testFullyConcurrentAddWithClear(dictionary, holder))));
            
            while (!System.Threading.Tasks.Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(1)))
            {
                GC.Collect();
            }

            Console.WriteLine("AddSuccess Count: {0}", MismatchResults.Count(result => result.AddSuccess));
            Console.WriteLine("ContainedKey Count: {0}", MismatchResults.Count(result => result.ContainedKey));
            Console.WriteLine("GetSuccess Count: {0}", MismatchResults.Count(result => result.GetSuccess));
            Console.WriteLine("GetMatched Count: {0}", MismatchResults.Count(result => result.GetMatched));
            Console.WriteLine("RemoveSuccess Count: {0}", MismatchResults.Count(result => result.RemoveSuccess));
            Console.WriteLine("RemoveMatched Count: {0}", MismatchResults.Count(result => result.RemoveMatched));
            Console.WriteLine("MatchesGetOrAdd Count: {0}", MismatchResults.Count(result => result.ObjMatchesGetOrAdd));
        }

        private void testFullyConcurrentAddNoGC(ConcurrentWeakRefDictionary<string, Obj> dictionary, Holder holder)
        {
            var addSuccess = dictionary.TryAdd(holder, holder);
            var containsKey = dictionary.ContainsKey(holder);

            bool getSuccess, removeSuccess, getMatched, removeMatched;
            NoAssertGet(dictionary, holder, out getSuccess, out getMatched);
            NoAssertRemove(dictionary, holder, out removeSuccess, out removeMatched);
            var objMatchesGetOrAdd = (holder.Obj == dictionary.GetOrAdd(holder, holder));
            ConcurrentResult result = new ConcurrentResult
                                          {
                                              AddSuccess = addSuccess,
                                              ContainedKey = containsKey,
                                              GetSuccess = getSuccess,
                                              GetMatched = getMatched,
                                              RemoveSuccess = removeSuccess,
                                              RemoveMatched = removeMatched,
                                              ObjMatchesGetOrAdd = objMatchesGetOrAdd
                                          };
            MismatchResults.Add(result);
        }

        private void testFullyConcurrentAddWithClear(ConcurrentWeakRefDictionary<string, Obj> dictionary, Holder holder)
        {
            var addSuccess = dictionary.TryAdd(holder, holder);
            holder.Clear();

            var containsKey = dictionary.ContainsKey(holder);

            bool getSuccess, removeSuccess, getMatched, removeMatched;
            NoAssertGet(dictionary, holder, out getSuccess, out getMatched);
            NoAssertRemove(dictionary, holder, out removeSuccess, out removeMatched);
            var objMatchesGetOrAdd = (holder.Obj == dictionary.GetOrAdd(holder, holder));
            ConcurrentResult result = new ConcurrentResult
            {
                AddSuccess = addSuccess,
                ContainedKey = containsKey,
                GetSuccess = getSuccess,
                GetMatched = getMatched,
                RemoveSuccess = removeSuccess,
                RemoveMatched = removeMatched,
                ObjMatchesGetOrAdd = objMatchesGetOrAdd
            };
            MismatchResults.Add(result);
        }

        private void AssertGet(bool expected, ConcurrentWeakRefDictionary<string, Obj> dictionary, Holder holder)
        {
            Obj temp;
            Assert.AreEqual(expected, dictionary.TryGetValue(holder, out temp));
            Assert.AreEqual(holder.Obj, temp);
        }

        private void AssertRemove(bool expected, ConcurrentWeakRefDictionary<string, Obj> dictionary, Holder holder)
        {
            Obj temp;
            Assert.AreEqual(expected, dictionary.TryRemove(holder, out temp));
            Assert.AreEqual(holder.Obj, temp);
        }

        private void NoAssertGet( ConcurrentWeakRefDictionary<string, Obj> dictionary, Holder holder, out bool getReturned, out bool outIsInput)
        {
            Obj temp;
            getReturned = dictionary.TryGetValue(holder, out temp);
            outIsInput = (holder.Obj == temp);
        }

        private void NoAssertRemove(ConcurrentWeakRefDictionary<string, Obj> dictionary, Holder holder, out bool removedReturn, out bool outIsInput)
        {
            Obj temp;
            removedReturn = dictionary.TryRemove(holder, out temp);
            outIsInput = (holder.Obj == temp);
        }

        private static string enumerableToString<T>(IEnumerable<T> input)
        {
            return input == null ? string.Empty : string.Join("\n", input.Select(item => item == null ? "{{null}}" : item.ToString()).ToArray());
        }
    }
}
