using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Collections.Generic;

namespace jQueryBuddy.Utilities
{
    public static class Loader
    {
        static readonly Dictionary<string, Assembly> Libs = new Dictionary<string, Assembly>();

        public static Assembly FindAssemblies(object sender, ResolveEventArgs args)
        {
            var shortName = new AssemblyName(args.Name).Name;
            if (Libs.ContainsKey(shortName)) return Libs[shortName];

            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("jQueryBuddy.Libs." + shortName + ".dll.gz"))
            {
                if (s == null) return null;

                // The following line would be for uncompressed assemblies
                //var data = new BinaryReader(s).ReadBytes((int) s.Length);

                var a = Assembly.Load(Decompress(s));
                Libs[shortName] = a;
                return a;
            }
        }

        private static byte[] Decompress(Stream compressed)
        {
            using (var stream = new GZipStream(compressed, CompressionMode.Decompress))
            {
                const int size = 4096;
                var buffer = new byte[size];
                using (var memory = new MemoryStream())
                {
                    var count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }
}