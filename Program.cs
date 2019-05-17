using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("First argument must be a url");
            }
            Console.WriteLine("Hello World!");

            var pointCloudReader = new PointCloudReader(args[0]);

            pointCloudReader.Analyze();

        }
    }

    public class PointCloudReader
    {
        private readonly string url;
        private PointCloud pointCloud;

        public PointCloudReader(string url)
        {
            this.url = url;

            using (var w = new WebClient())
            {
                var json = string.Empty;


                try
                {
                    json = w.DownloadString(url);

                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (!string.IsNullOrEmpty(json))
                {
                    this.pointCloud = JsonConvert.DeserializeObject<PointCloud>(json);
                }
            }
        }
        public int[] Analyze()
        {
            if (pointCloud == null)
            {
                return new int[0];
            }

            var smallest = findSmallest(pointCloud.ReferenceArray);
            //print(pointCloud.ReferenceArray);
            var arry = shiftArray(pointCloud.ReferenceArray, smallest);
            //print(arry);

            Console.WriteLine("Reference ***************");
            var referenceNumbers = calculateNumbers(arry);
            print(referenceNumbers);

            Console.WriteLine("***************");

            int count = 0;
            foreach (var pc in pointCloud.PointCloudList)
            {
                Console.WriteLine($"{count++} ***************");
                var sm = findSmallest(pc);
                var ar = shiftArray(pc, sm);
                var nms = calculateNumbers(ar);
                print(nms);

                ar = rotate(pc);
                sm = findSmallest(ar);
                ar = shiftArray(ar, sm);
                nms = calculateNumbers(ar);
                print(nms);

                ar = rotate(pc);
                ar = rotate(ar);
                sm = findSmallest(ar);
                ar = shiftArray(ar, sm);
                nms = calculateNumbers(ar);
                print(nms);

                ar = rotate(pc);
                ar = rotate(ar);
                ar = rotate(ar);
                sm = findSmallest(ar);
                ar = shiftArray(ar, sm);
                nms = calculateNumbers(ar);
                print(nms);
            }




            return new int[0];
        }

        private int[][] rotate(int[][] matrix, int n = 8)
        {
            int[][] ret = new int[n][];
            for (int i = 0; i < 8; i++)
            {
                ret[i] = new int[8];
            }

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i][j] = matrix[n - j - 1][i];
                }
            }

            return ret;
        }

        public int findSmallest(int[][] array)
        {

            int smallest = 8;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (array[x][y] == 1)
                    {
                        smallest = x;
                    }

                    if (x >= smallest)
                    {
                        break;
                    }
                }
            }

            return smallest;
        }
        public int[] calculateNumbers(int[][]array)
        {
            var result = new int[8];

            for(int y = 0; y < 8; y++)
            {
                var sb = new StringBuilder();
                for (int x = 0; x<8;x++)
                {
                    sb.Append(array[x][y]);
                }
                result[y] = Convert.ToInt32(sb.ToString(), 2);
            }

            return result;
        }

        public void print (int[] numbers)
        {
            for (int x = 0; x < 8; x++)
            {
                Console.Out.Write($"{numbers[x]}\t");
            }


            Console.Out.WriteLine();
        }

        public void print(int[][] array)
        {
            for(int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Console.Out.Write($"{array[x][y]}\t");
                }
                Console.Out.WriteLine();
            }

            Console.Out.WriteLine("****");
        } 

        public int[][] shiftArray( int[][] array, int shift)
        {
            var arrayShifted = new int[8][];
            for (int i = 0; i < 8; i++)
            {
                arrayShifted[i] = new int[8];
            }


            for (int row = 0; row < array.Length; row++)
            {
                for (int col = 0; col < array[row].Length; col++)
                {
                    arrayShifted[row][col] = array[(row + shift) % array[col].Length][col];
                }
            }

            return arrayShifted;
        }
    }

    public class PointCloud
    {
        public string Reference { get; set; }

        private int[][] _referenceArray;

        public int[][] ReferenceArray
        {
            get
            {
                if (_referenceArray == null)
                {
                    _referenceArray = new int[8][];

                    for(int i = 0; i < 8; i++)
                    {
                        _referenceArray[i] = new int[8];
                    }


                    int x = 0;
                    int y = 0;
                    foreach (var s in Reference.Split(' '))
                    {
                        var cArray = s.ToCharArray();
                        foreach (var c in cArray)
                        {
                            _referenceArray[x++][y] = c != '0' ? 1 : 0;
                        }

                        y++;
                        x = 0;
                    }

                }

                return _referenceArray;
            }
        }

        public string[] PointClouds { get; set; }

        private List<int[][]> _pointCloudList;


        public List<int[][]> PointCloudList
        {
            get
            {
                if (_pointCloudList == null)
                {
                    _pointCloudList = new List<int[][]>();

                    foreach (var z in PointClouds)
                    {

                        var arry = new int[8][];
                        for (int i = 0; i < 8; i++)
                        {
                            arry[i] = new int[8];
                        }

                        int x = 0;
                        int y = 0;
                        foreach (var s in z.Split(' '))
                        {
                            var cArray = s.ToCharArray();
                            foreach (var c in cArray)
                            {
                                arry[x++][y] = c != '0' ? 1 : 0;
                            }

                            y++;
                            x = 0;
                        }
                        _pointCloudList.Add(arry);
                    }

                }

                return _pointCloudList;

            }
        }
    }
}
