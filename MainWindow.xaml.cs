using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using LLVMSharp;

namespace LanguageNet5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var module = LLVM.ModuleCreateWithName("TestModule");
            var builder = LLVM.CreateBuilder();


            var funcImpl = LLVM.AddFunction(module, "main", LLVM.FunctionType(LLVM.Int32Type(), new LLVMTypeRef[] { }, false));
            LLVM.SetLinkage(funcImpl, LLVMLinkage.LLVMExternalLinkage);

            LLVM.PositionBuilderAtEnd(builder, LLVM.AppendBasicBlock(funcImpl, "entry"));
            var val = LLVM.ConstInt(LLVM.Int32Type(), 500, false); ;
            /*var add = LLVM.BuildAdd(builder, val, val, "val2");
            var ld = LLVM.BuildLoad(builder, LLVM.BuildIntToPtr(builder, val, LLVMTypeRef.PointerType(LLVM.Int32Type(), 0), "ptr"), "ld");
            var div = LLVM.BuildSDiv(builder, add, ld, "div");*/

            LLVM.BuildRet(builder, val);

            LLVM.VerifyFunction(funcImpl, LLVMVerifierFailureAction.LLVMPrintMessageAction);

            var ptr = LLVM.PrintModuleToString(module);
            var mod = Marshal.PtrToStringAnsi(ptr);
            Text.Text = mod;

            LLVM.WriteBitcodeToFile(module, "test.bc");

            var clang = Process.Start("clang", "test.bc -o test.exe");
            clang.WaitForExit();
        }
    }
}
