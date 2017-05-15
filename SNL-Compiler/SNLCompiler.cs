using System;
using System.IO;

namespace SNL_Compiler
{
    class SNLCompiler
    {
        static void Main(string[] args)
        {
            string filePath = "test.snl";
            string fileContent = File.ReadAllText(filePath);//ReadAllText
            fileContent = fileContent.Replace("\r", "");//delete \r

            //DoToken
            string fileOutContent = "";
            DoToken.doToken(fileContent);
            fileOutContent = Data.tokenShow;
            Console.WriteLine(fileOutContent);
            filePath = "token.out";
            File.Delete(filePath);
            File.WriteAllText(filePath, fileOutContent);


            /* 在语法分析的结果中，每个单词前都用圆括号包含为了匹配到该终极符而规约的SNL对应下标的规则
               为了简单起见，语法分析时直接将所有标识符统一表示为ID，将所有数字常量统一表示为INTC。 */
            //DoGrammar LL（1）分析法
            Data.initialize();
            fileOutContent = DoGrammar.doGrammar();
            Console.WriteLine(fileOutContent);
            filePath = "parse.out";
            File.Delete(filePath);
            File.WriteAllText(filePath, fileOutContent);

            //Another DoGrammar 递归下降法
            Recursion recursion = new Recursion();
            fileOutContent = recursion.stree;
            Console.WriteLine(fileOutContent);
            filePath = "parse2.out";
            File.Delete(filePath);
            File.WriteAllText(filePath, fileOutContent);
        }
    }
}
