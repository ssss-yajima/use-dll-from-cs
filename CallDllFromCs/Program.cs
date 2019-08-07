using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CallDllFromCs
{
    class Program
    {
        #region DllImport
        [DllImport("MyDll.dll")]
        private static extern int MyFuncA(int a);

        [DllImport("MyDll.dll")]
        private static extern void MyFuncB(int a, string str);

        [DllImport("MyDll.dll")]
        //char[]を操作する関数に対してStringBuilderを渡す
        private static extern void MyFuncC(int a, StringBuilder str);

        [DllImport("MyDll.dll")]
        private static extern void MyFuncD(SampleStruct st);

        [DllImport("MyDll.dll")]
        //SampleStruct*に対してIntPtrを渡す
        private static extern void MyFuncE(IntPtr pst); //IntPtrを渡すパターン：OK
        [DllImport("MyDll.dll")]
        private static extern void MyFuncE(SampleStructClass st); //参照型を渡すパターン：NG

        [DllImport("MyDll.dll")]
        //ポインタを含む構造体SampleStruct2を渡す
        private static extern void MyFuncF(IntPtr pst2);

        [DllImport("MyDll.dll")]
        //intのポインタを渡す
        private static extern void MyFuncG(ref int a);

        [DllImport("MyDll.dll")]
        //intのポインタのポインタを渡す
        unsafe private static extern void MyFuncH(int** a);

        [DllImport("MyDll.dll")]
        //構造体SampleStructのポインタのポインタを渡す
        unsafe private static extern void MyFuncI(ref IntPtr ppst);

        #endregion
        #region 構造体定義
        //構造体
        [StructLayout(LayoutKind.Sequential)]
        private struct SampleStruct
        {
            //4バイト符号付整数
            //[MarshalAs(UnmanagedType.I4)] //Blitableでは不要？なくても正常に動く。
            public int index; //変数名は元の構造体と同じである必要はない。型、大きさ、順序が問題。

            //固定長文字列配列
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string name;

            //固定長配列
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
            public int[] data;
        }

        //ポインタを含む構造体
        [StructLayout(LayoutKind.Sequential)]
        private struct SampleStruct2
        {
            public int length;
            public IntPtr data; //元のC++はdouble*
        }
        //ポインタのポインタを渡すため参照型であるクラスとして定義
        [StructLayout(LayoutKind.Sequential)]
        private class SampleStructClass
        {
            //4バイト符号付整数
            //[MarshalAs(UnmanagedType.I4)] //Blitableでは不要？なくても正常に動く。
            public int index; //変数名は元の構造体と同じである必要はない。型、大きさ、順序が問題。

            //固定長文字列配列
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string name;

            //固定長配列
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
            public int[] data;
        }

        #endregion

        static void Main(string[] args)
        {
            #region MyFuncA intを渡す
            int ans = MyFuncA(1);
            Console.WriteLine(ans);
            #endregion
            #region MyFuncB　char[]を渡す
            MyFuncB(2, "call MyFuncB from C#");
            #endregion
            #region MyFuncC char[]を渡して操作させる
            StringBuilder sb = new StringBuilder(256);
            MyFuncC(3, sb);
            //領域不足だとAccessViolationException
            //MyFuncC(4, new StringBuilder(4)); 
            #endregion
            #region MyFuncD 構造体を渡す
            //渡す構造体の初期化
            var st = new SampleStruct()
            {
                index = 4,
                name = "構造体サンプル",
                data = new int[50],
            };
            st.data[0] = 10;
            st.data[1] = 20;
            st.data[2] = 30;
            MyFuncD(st);
            #endregion
            #region MyFuncE 構造体を渡して操作させる
            var pst = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SampleStruct)));
            try
            {
                MyFuncE(pst);
                var st_rtn = (SampleStruct)Marshal.PtrToStructure(pst, typeof(SampleStruct));
                Console.WriteLine($"index = {st_rtn.index}");
                Console.WriteLine($"name = {st_rtn.name}");
                Console.WriteLine(($"data = [{st_rtn.data[0]},{st_rtn.data[1]},{st_rtn.data[2]}]"));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                //必ずメモリ解放
                Marshal.FreeHGlobal(pst);
            }
            #region 参照型の参照渡し・・・正常終了するが値がセットされない
            Console.WriteLine("MyFuncE(参照型を渡すパターン)");
            SampleStructClass stc = new SampleStructClass();
            MyFuncE(stc);
            Console.WriteLine($"index = {stc.index}");
            Console.WriteLine($"name = {stc.name}");
            //Console.WriteLine($"data = {stc.data[0]}, {stc.data[1]}, {stc.data[2]}");
            #endregion
            #endregion
            #region MyFuncF ポインタを含む構造体を渡す
            var pst2 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SampleStruct2)));
            try
            {
                MyFuncF(pst2);
                var st2 = (SampleStruct2)Marshal.PtrToStructure(pst2, typeof(SampleStruct2));
                for(var i=0; i<st2.length; i++)
                {
                    var v = Marshal.ReadInt64(st2.data, i * sizeof(double));
                    Console.WriteLine($"data[{i}] = {BitConverter.Int64BitsToDouble(v)}");
                }
            }catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                Marshal.FreeHGlobal(pst2);
            }
            #endregion
            #region MyFuncG intのポインタを渡す
            int g = 0;
            Console.WriteLine(g);
            MyFuncG(ref g);
            #endregion
            #region MyFuncH intのポインタのポインタを渡す
            unsafe
            {
                int h = 0;
                int* pt1 = &h;
                int** pt2 = &pt1;
                Console.WriteLine(**pt2);
                MyFuncH(pt2);
            }
            #endregion
            #region MyFuncI 構造体SampleStructのポインタのポインタを渡す
            var pst3 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SampleStruct)));
            try
            {
                MyFuncI(ref pst3);
                var st_rtn = (SampleStruct)Marshal.PtrToStructure(pst3, typeof(SampleStruct));
                Console.WriteLine($"index = {st_rtn.index}");
                Console.WriteLine($"name = {st_rtn.name}");
                Console.WriteLine(($"data = [{st_rtn.data[0]},{st_rtn.data[1]},{st_rtn.data[2]}]"));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                //必ずメモリ解放
                Marshal.FreeHGlobal(pst);
            }
            #endregion
            Console.ReadKey();
        }
    }
}