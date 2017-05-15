using System;
using System.Collections.Generic;

//LL（1）分析法

namespace SNL_Compiler
{
    class DoGrammar
    {
        public static Stack<string> stack;
        public static string grammarShow;
        public static string match;

        public static string doGrammar()
        {
            grammarShow = "";
            match = "";
            match += ("(");
            stack = new Stack<string>();
            stack.Push("Program");
            string iToken = null; // 为token序列首位的还原后的非终极符
            string iStack = null; // 为栈顶非终极符
            int iRule = 0; // 为规约用的规则行号
            int iLine = 1; // token中的行号
            int i = 0;
            while (i < Data.token.Count)
            {
                if (Data.token[i].line > iLine)
                {
                    iLine = Data.token[i].line;
                    grammarShow += "\n";
                }
                // 还原token中的单词信息至iToken
                switch (Data.token[i].lex)
                {
                    case 1:
                        iToken = Data.token[i].sem;
                        break;
                    case 2:
                        iToken = Data.token[i].sem;
                        break;
                    case 3:
                        iToken = "ID"; // 赋值为ID，即标识符
                        break;
                    case 4:
                        iToken = "INTC"; // 赋值为INTC，即数字常量
                        break;
                }
                if (stack.Count == 0)
                { // 如果token序列还未遍历完而栈已空，则出错
                    grammarShow += "Error: line" + iLine + " : stack is empty but token is not empty\n";//栈已空而token序列不为空
                    return grammarShow;
                }
                iStack = stack.Peek(); // 为栈顶元素
                stack.Pop();
                if (iStack.Equals("null"))
                {
                    continue;
                }
                if (Data.nonTerminal.Contains(iStack))
                { // 栈顶元素为非终极符，则要根据LL(1)分析表进行规约
                    if ((iRule = Data.analysis[Data.nonTerminal.IndexOf(iStack),Data.terminal.IndexOf(iToken)]) == 0)
                    { // 出错！因没有可用的规则进行规约
                        grammarShow += "Error: line" + iLine + " : no rules found\n";//进行规约时出错：无可用的规则
                        return grammarShow;
                    }
                    else
                    {
                        for (int j = Data.rule[iRule].B.Count - 1; j >= 0; j--)
                        { // 将规则逆序入栈
                            stack.Push(Data.rule[iRule].B[j]);
                        }
                        match += iRule + ",";
                        continue;
                    }
                }
                else
                { // 此时栈顶元素为终极符，则要跟token序列首位进行匹配
                    if (iStack.Equals(iToken))
                    { // 匹配成功
                        match = match.Remove(match.Length - 1,1); // 删掉末尾的（或，
                        if (match.Length != 0)
                        {
                            match += (")"); // 如果不空则追加上）
                        }
                        grammarShow += match + iToken + " ";
                        match = "";
                        match += "(";
                        i++;
                        continue;
                    }
                    else
                    { // 不匹配则出错
                        grammarShow += "Error: line" + iLine + " : error while matching the terminal\n";//进行终极符匹配时出错
                        return grammarShow;
                    }
                }
            } // while循环结束
            if (stack.Count == 0)
                grammarShow += "\nDoGrammar Over!\n";
            else
                grammarShow += "Error: line" + iLine + " : token is empty but stack is not empty\n";//token序列已为空而栈不空
            return grammarShow;
        }
    }
}
