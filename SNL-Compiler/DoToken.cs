using System;
using System.Collections.Generic;

namespace SNL_Compiler
{
    class DoToken
    {
        public static List<string> identifier; // 标识符列表
        public static List<string> INTC; // 常量列表

        public static Boolean isIdentifier(string s)
        { // 标识符自动机
            if (!Data.letter.Contains(s[0]))
            {
                return false;
            }
            for (int i = 1; i < s.Length; i++)
            {
                if (!Data.letter.Contains(s[i]) && !Data.digit.Contains(s[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public static Boolean isINTC(string s)
        { // 数字常量自动机
            for (int i = 0; i < s.Length; i++)
            {
                if (!Data.digit.Contains(s[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void doToken(string s)
        { // 词法分析主方法
            identifier = new List<string>(); // 初始化各列表和缓冲字符串
            INTC = new List<string>();
            Data.token = new List<Token>();
            Data.tokenShow = "";
            int line = 1; // 行数初始化为一
            string ss; // s是全部的字符串，ss是分隔出的字符串
            string str = ""; // str用来保存分隔中的字符串
            Token t; // 新建的token对象
            for (int i = 0; i < s.Length; i++)
            { // 从源程序中一个字符一个字符地进行读取，并逐个分离出单词，然后构造它们的机内表示Token
                if (s[i] != ' ' && s[i] != '\n' && s[i] != '\t'
                        && !Data.separator.Contains(Convert.ToString(s[i])))
                { // 如果该字符不是分隔符则直接追加到str中
                    str += s[i];
                }
                else
                { // 如果该字符是分隔符
                  // 处理分离出的单词
                    if (str.Length != 0)
                    { // 若分离出的单词长度为零，则跳过此部分而直接进入后面的分隔符生成部分
                        ss = str;
                        if (Data.letter.Contains(ss[0]))
                        {
                            if (Data.reservedWord.Contains((ss)))
                            { // 如果分隔出的字符串是保留字则在token和tokenShow都要追加
                                t = new Token(2, line, ss);
                                Data.token.Add(t);
                                Data.tokenShow += line + " reserved word, " + ss + "\n";
                            }
                            else if (isIdentifier(ss))
                            { // 如果分隔出的字符串是标识符则在token和tokenShow都要追加
                                if (!identifier.Contains(ss))
                                {
                                    identifier.Add(ss); // 如果标识符列表中没有该标识符则添加
                                }
                                t = new Token(3, line, ss);
                                Data.token.Add(t);
                                Data.tokenShow += line + " ID, name = " + ss + "\n";
                            }
                            else
                            {
                                Data.tokenShow += "Error：line" + line + " ： " + ss + "\n";
                            }
                        }
                        else
                        { // 如果是数字常量
                            if (isINTC(ss))
                            {
                                if (!INTC.Contains(ss))
                                { // 如果数字常量列表中没有该数字常量则添加
                                    INTC.Add(ss);
                                }
                                t = new Token(4, line, ss);
                                Data.token.Add(t);
                                Data.tokenShow += line + " INTC, val = " + ss + "\n";
                            }
                            else
                            {
                                Data.tokenShow += "Error：line" + line + " ： " + ss + "\n";
                            }
                        }
                        str = ""; // 重新初始化用以分离单词的缓冲字符串
                    }
                    // 处理分隔符
                    if (s[i] == ' ')
                    { // 如果分隔符是空格
                        continue;
                    }
                    if (s[i] == '\n')
                    { // 如果分隔符是换行符
                        line++; // 将行号+1
                        continue;
                    }
                    if (s[i] == '\t')
                    { // 如果分隔符是制表符
                        continue;
                    }
                    if (s[i] == ':')
                    {
                        if (s[++i] == '=')
                        {
                            t = new Token(1, line, ":="); // 如果分隔符是赋值符则在token和tokenShow都要追加
                            Data.token.Add(t);
                            Data.tokenShow += line + " :=\n";
                            continue;
                        }
                        else
                        {
                            Data.tokenShow += "Error：line" + line + " ： " + "= should be followed with :" + "\n";
                        }
                    }
                    if (s[i] == '.')
                    {
                        if ((i + 1) != s.Length && s[i + 1] == '.')
                        { // 如果是..则在token和tokenShow都要追加
                            t = new Token(1, line, ".."); // 如果分隔符是数组间符则在token和tokenShow都要追加
                            Data.token.Add(t);
                            Data.tokenShow += line + " ..\n";
                            i++;
                            continue;
                        }
                        // 如果分隔符是域作用符或程序结束符则在token和tokenShow都要追加
                        t = new Token(1, line, ".");
                        Data.token.Add(t);
                        Data.tokenShow += line + " .\n";
                        if ((i + 1) == s.Length || s[i + 1] == ' ' || s[i + 1] == '\n' || s[i + 1] == '\t')
                        { // 如果已到程序末尾或未达末尾但其后字符为空格、回车或制表符则结束词法分析
                            Data.tokenShow += "DoToken Over!\n";
                            return;
                        }
                        continue;
                    }
                    t = new Token(1, line, Convert.ToString(s[i])); // 如果分隔符是赋值符则在token和tokenShow都要追加
                    Data.token.Add(t);
                    Data.tokenShow += line + " " + s[i] + "\n";
                }
            }
            if (str.Length != 0)
            { // 若程序结束时分离出的单词长度不为零，则处理为相应的Token（尽管词法分析已失败，因为程序未能成功结束）
                ss = str;
                if (Data.letter.Contains(ss[0]))
                {
                    if (Data.reservedWord.Contains((ss)))
                    { // 如果分隔出的字符串是保留字则在token和tokenShow都要追加
                        t = new Token(2, line, ss);
                        Data.token.Add(t);
                        Data.tokenShow += line + ss.ToUpper() + ", " + ss + "\n";
                    }
                    else if (isIdentifier(ss))
                    { // 如果分隔出的字符串是标识符则在token和tokenShow都要追加
                        if (!identifier.Contains(ss))
                        {
                            identifier.Add(ss); // 如果标识符列表中没有该标识符则添加
                        }
                        t = new Token(3, line, ss);
                        Data.token.Add(t);
                        Data.tokenShow += line + " ID, name = " + ss + "\n";
                    }
                    else
                    {
                        // 用*号行标识的代码行为方法的可能出口
                        Data.tokenShow += "Error：line" + line + " ： " + ss + "\n";
                    }
                }
                else
                { // 如果是数字常量
                    if (isINTC(ss))
                    {
                        if (!INTC.Contains(ss))
                        { // 如果数字常量列表中没有该数字常量则添加
                            INTC.Add(ss);
                        }
                        t = new Token(4, line, ss);
                        Data.token.Add(t);
                        Data.tokenShow += line + " INTC, val = " + ss + ")";
                    }
                    else
                    {
                        Data.tokenShow += "Error：line" + line + " ： " + ss + "\n";
                    }
                }
                str = ""; // 重新初始化用以分离单词的缓冲字符串
            }
            Data.tokenShow += "Error：line" + line + " ： " + "The program is not finished normally." + "\n";//程序未正常结束
        }
    }
}
