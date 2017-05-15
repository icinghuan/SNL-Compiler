using System;
using System.Collections.Generic;

namespace SNL_Compiler
{
    class Token // Token类
    {
        public int lex; // i表示类型：1为分隔符，2为保留字，3为标识符，4为数字常量
        public int line; // 行号
        public string sem; // 语义

        public Token(int lex, int line, string sem)
        {
            this.lex = lex;
            this.line = line;
            this.sem = sem;
        }
    }

    class Rule
    { // SNL的语法规则
        public string A;
        public List<string> B = new List<string>();
    }

    class Data
    {
        public static string tokenShow = ""; // 显示token用string
        public static List<Token> token; // token列表
        // 分隔符列表
        public static List<string> separator = new List<string> { ",", ";", "+", "-", "*", "/", "<", "=", "(", ")", "[", "]", ":=", ".", "..", ":" };
        // 保留字列表
        public static List<string> reservedWord = new List<string> { "program", "type", "var", "integer", "char", "array", "of", "procedure", "begin",
                                                        "while", "do", "if", "then", "else", "fi", "endwh", "end", "read", "write", "return" };
        // 大小写字母表
        public static List<char> letter = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                                                'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                                                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
        // 0-9
        public static List<char> digit = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        // 终极符
        public static List<string> terminal = new List<string> { "null", "program", "type", "integer", "char", "array", "INTC", "record", "end", "var",
                                                    "procedure", "begin", "if", "then", "else", "fi", "while", "do", "endwh", "read",
                                                    "write", "return", "ID", ".", ";", ",", "(", ")", "[", "]", "<", "=", "+", "-", "*", "/", ":=" };
        // 非终极符
        public static List<string> nonTerminal = new List<string> { "null", "Program", "ProgramHead", "ProgramName", "DeclarePart", "TypeDecpart",
                                                    "TypeDec", "TypeDecList", "TypeDecMore", "TypeId", "TypeDef", "BaseType", "StructureType",
                                                    "ArrayType", "Low", "Top", "RecType", "FieldDecList", "FieldDecMore", "IdList", "IdMore",
                                                    "VarDecpart", "VarDec", "VarDecList", "VarDecMore", "VarIdList", "VarIdMore", "ProcDecpart",
                                                    "ProcDec", "ProcDecMore", "ProcName", "ParamList", "ParamDecList", "ParamMore", "Param", "FormList",
                                                    "FidMore", "ProcDecPart", "ProcBody", "ProgramBody", "StmList", "StmMore", "Stm", "AssCall", "AssignmentRest",
                                                    "ConditionalStm", "LoopStm", "InputStm", "Invar", "OutputStm", "ReturnStm", "CallStmRest", "ActParamList",
                                                    "ActParamMore", "RelExp", "OtherRelE", "Exp", "OtherTerm", "Term", "OtherFactor", "Factor", "Variable", "VariMore",
                                                    "FieldVar", "FieldVarMore", "CmpOp", "AddOp", "MulOp" };

        public static int[,] analysis = new int[68,37]; // LL(1)分析表
        public static List<Rule> rule = new List<Rule>(); // 存放SNL的104条规则

        public static void initialize()
        { // 初始化各个列表


            analysis[1,1] = 1;
            analysis[2,1] = 2;
            analysis[3,22] = 3;

            analysis[4,2] = 4;
            analysis[4,9] = 4;
            analysis[4,10] = 4;
            analysis[4,11] = 4;

            analysis[5,2] = 6;
            analysis[5,9] = 5;
            analysis[5,10] = 5;
            analysis[5,11] = 5;

            analysis[6,2] = 7;
            analysis[7,22] = 8;

            analysis[8,9] = 9;
            analysis[8,10] = 9;
            analysis[8,11] = 9;
            analysis[8,22] = 10;

            analysis[9,22] = 11;

            analysis[10,3] = 12;
            analysis[10,4] = 12;
            analysis[10,5] = 13;
            analysis[10,7] = 13;
            analysis[10,22] = 14;

            analysis[11,3] = 15;
            analysis[11,4] = 16;

            analysis[12,5] = 17;
            analysis[12,7] = 18;

            analysis[13,5] = 19;
            analysis[14,6] = 20;
            analysis[15,6] = 21;
            analysis[16,7] = 22;

            analysis[17,3] = 23;
            analysis[17,4] = 23;
            analysis[17,5] = 24;

            analysis[18,3] = 26;
            analysis[18,4] = 26;
            analysis[18,5] = 26;
            analysis[18,8] = 25;

            analysis[19,22] = 27;

            analysis[20,24] = 28;
            analysis[20,25] = 29;

            analysis[21,9] = 31;
            analysis[21,10] = 30;
            analysis[21,11] = 30;

            analysis[22,9] = 32;

            analysis[23,3] = 33;
            analysis[23,4] = 33;
            analysis[23,5] = 33;
            analysis[23,7] = 33;
            analysis[23,22] = 33;

            analysis[24,3] = 35;
            analysis[24,4] = 35;
            analysis[24,5] = 35;
            analysis[24,7] = 35;
            analysis[24,10] = 34;
            analysis[24,11] = 34;
            analysis[24,22] = 35;

            analysis[25,22] = 36;

            analysis[26,24] = 37;
            analysis[26,25] = 38;

            analysis[27,10] = 40;
            analysis[27,11] = 39;

            analysis[28,10] = 41;

            analysis[29,10] = 43;
            analysis[29,11] = 42;

            analysis[30,22] = 44;

            analysis[31,3] = 46;
            analysis[31,4] = 46;
            analysis[31,5] = 46;
            analysis[31,7] = 46;
            analysis[31,9] = 46;
            analysis[31,22] = 46;
            analysis[31,27] = 45;

            analysis[32,3] = 47;
            analysis[32,4] = 47;
            analysis[32,5] = 47;
            analysis[32,7] = 47;
            analysis[32,9] = 47;
            analysis[32,22] = 47;

            analysis[33,24] = 49;
            analysis[33,27] = 48;

            analysis[34,3] = 50;
            analysis[34,4] = 50;
            analysis[34,5] = 50;
            analysis[34,7] = 50;
            analysis[34,9] = 51;
            analysis[34,22] = 50;

            analysis[35,22] = 52;

            analysis[36,24] = 53;
            analysis[36,25] = 54;
            analysis[36,27] = 53;

            analysis[37,2] = 55;
            analysis[37,9] = 55;
            analysis[37,10] = 55;
            analysis[37,11] = 55;

            analysis[38,11] = 56;
            analysis[39,11] = 57;

            analysis[40,12] = 58;
            analysis[40,16] = 58;
            analysis[40,19] = 58;
            analysis[40,20] = 58;
            analysis[40,21] = 58;
            analysis[40,22] = 58;

            analysis[41,8] = 59;
            analysis[41,14] = 59;
            analysis[41,15] = 59;
            analysis[41,18] = 59;
            analysis[41,24] = 60;

            analysis[42,12] = 61;
            analysis[42,16] = 62;
            analysis[42,19] = 63;
            analysis[42,20] = 64;
            analysis[42,21] = 65;
            analysis[42,22] = 66;

            analysis[43,23] = 67;
            analysis[43,26] = 68;
            analysis[43,28] = 67;
            analysis[43,36] = 67;

            analysis[44,23] = 69;
            analysis[44,28] = 69;
            analysis[44,36] = 69;

            analysis[45,12] = 70;
            analysis[46,16] = 71;
            analysis[47,19] = 72;
            analysis[48,22] = 73;
            analysis[49,20] = 74;
            analysis[50,21] = 75;
            analysis[51,26] = 76;

            analysis[52,6] = 78;
            analysis[52,22] = 78;
            analysis[52,26] = 78;
            analysis[52,27] = 77;

            analysis[53,25] = 80;
            analysis[53,27] = 79;

            analysis[54,6] = 81;
            analysis[54,22] = 81;
            analysis[54,26] = 81;

            analysis[55,30] = 82;
            analysis[55,31] = 82;

            analysis[56,6] = 83;
            analysis[56,22] = 83;
            analysis[56,26] = 83;

            analysis[57,8] = 84;
            analysis[57,13] = 84;
            analysis[57,14] = 84;
            analysis[57,15] = 84;
            analysis[57,17] = 84;
            analysis[57,18] = 84;
            analysis[57,24] = 84;
            analysis[57,25] = 84;
            analysis[57,27] = 84;
            analysis[57,29] = 84;
            analysis[57,30] = 84;
            analysis[57,31] = 84;
            analysis[57,32] = 85;
            analysis[57,33] = 85;

            analysis[58,6] = 86;
            analysis[58,22] = 86;
            analysis[58,26] = 86;

            analysis[59,8] = 87;
            analysis[59,13] = 87;
            analysis[59,14] = 87;
            analysis[59,15] = 87;
            analysis[59,17] = 87;
            analysis[59,18] = 87;
            analysis[59,24] = 87;
            analysis[59,25] = 87;
            analysis[59,27] = 87;
            analysis[59,29] = 87;
            analysis[59,30] = 87;
            analysis[59,31] = 87;
            analysis[59,32] = 87;
            analysis[59,33] = 87;
            analysis[59,34] = 88;
            analysis[59,35] = 88;

            analysis[60,6] = 90;
            analysis[60,22] = 91;
            analysis[60,26] = 89;

            analysis[61,22] = 92;

            analysis[62,8] = 93;
            analysis[62,13] = 93;
            analysis[62,14] = 93;
            analysis[62,15] = 93;
            analysis[62,17] = 93;
            analysis[62,18] = 93;
            analysis[62,23] = 95;
            analysis[62,24] = 93;
            analysis[62,25] = 93;
            analysis[62,27] = 93;
            analysis[62,28] = 94;
            analysis[62,29] = 93;
            analysis[62,30] = 93;
            analysis[62,31] = 93;
            analysis[62,32] = 93;
            analysis[62,33] = 93;
            analysis[62,34] = 93;
            analysis[62,35] = 93;
            analysis[62,36] = 93;

            analysis[63,22] = 96;

            analysis[64,8] = 97;
            analysis[64,13] = 97;
            analysis[64,14] = 97;
            analysis[64,15] = 97;
            analysis[64,17] = 97;
            analysis[64,18] = 97;
            analysis[64,24] = 97;
            analysis[64,25] = 97;
            analysis[64,27] = 97;
            analysis[64,28] = 98;
            analysis[64,29] = 97;
            analysis[64,30] = 97;
            analysis[64,31] = 97;
            analysis[64,32] = 97;
            analysis[64,33] = 97;
            analysis[64,34] = 97;
            analysis[64,35] = 97;
            analysis[64,36] = 97;

            analysis[65,30] = 99;
            analysis[65,31] = 100;

            analysis[66,32] = 101;
            analysis[66,33] = 102;

            analysis[67,34] = 103;
            analysis[67,35] = 104;

            Rule r = new Rule(); // 填充下标为零的规则
            r.A = "SNL";
            r.B.Add(".");
            rule.Add(r);

            r = new Rule();
            r.A = "Program";
            r.B.Add("ProgramHead");
            r.B.Add("DeclarePart");
            r.B.Add("ProgramBody");
            r.B.Add(".");
            rule.Add(r);

            r = new Rule();
            r.A = "ProgramHead";
            r.B.Add("program");
            r.B.Add("ProgramName");
            rule.Add(r);

            r = new Rule();
            r.A = "ProgramName";
            r.B.Add("ID");
            rule.Add(r);

            r = new Rule();
            r.A = "DeclarePart";
            r.B.Add("TypeDecpart");
            r.B.Add("VarDecpart");
            r.B.Add("ProcDecpart");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeDecpart";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeDecpart";
            r.B.Add("TypeDec");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeDec";
            r.B.Add("type");
            r.B.Add("TypeDecList");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeDecList";
            r.B.Add("TypeId");
            r.B.Add("=");
            r.B.Add("TypeDef");
            r.B.Add(";");
            r.B.Add("TypeDecMore");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeDecMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeDecMore";
            r.B.Add("TypeDecList");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeId";
            r.B.Add("ID");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeDef";
            r.B.Add("BaseType");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeDef";
            r.B.Add("StructureType");
            rule.Add(r);

            r = new Rule();
            r.A = "TypeDef";
            r.B.Add("ID");
            rule.Add(r);

            r = new Rule();
            r.A = "BaseType";
            r.B.Add("integer");
            rule.Add(r);

            r = new Rule();
            r.A = "BaseType";
            r.B.Add("char");
            rule.Add(r);

            r = new Rule();
            r.A = "StructureType";
            r.B.Add("ArrayType");
            rule.Add(r);

            r = new Rule();
            r.A = "StructureType";
            r.B.Add("RecType");
            rule.Add(r);

            r = new Rule();
            r.A = "ArrayType";
            r.B.Add("array");
            r.B.Add("[");
            r.B.Add("Low");
            r.B.Add("..");
            r.B.Add("Top");
            r.B.Add("]");
            r.B.Add("of");
            r.B.Add("BaseType");
            rule.Add(r);

            r = new Rule();
            r.A = "Low";
            r.B.Add("INTC");
            rule.Add(r);

            r = new Rule();
            r.A = "Top";
            r.B.Add("INTC");
            rule.Add(r);

            r = new Rule();
            r.A = "RecType";
            r.B.Add("record");
            r.B.Add("FieldDecList");
            r.B.Add("end");
            rule.Add(r);

            r = new Rule();
            r.A = "FieldDecList";
            r.B.Add("BaseType");
            r.B.Add("IdList");
            r.B.Add(";");
            r.B.Add("FieldDecMore");
            rule.Add(r);

            r = new Rule();
            r.A = "FieldDecList";
            r.B.Add("ArrayType");
            r.B.Add("IdList");
            r.B.Add(";");
            r.B.Add("FieldDecMore");
            rule.Add(r);

            r = new Rule();
            r.A = "FieldDecMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "FieldDecMore";
            r.B.Add("FieldDecList");
            rule.Add(r);

            r = new Rule();
            r.A = "IdList";
            r.B.Add("ID");
            r.B.Add("IdMore");
            rule.Add(r);

            r = new Rule();
            r.A = "IdMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "IdMore";
            r.B.Add(",");
            r.B.Add("IdList");
            rule.Add(r);

            r = new Rule();
            r.A = "VarDecpart";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "VarDecpart";
            r.B.Add("VarDec");
            rule.Add(r);

            r = new Rule();
            r.A = "VarDec";
            r.B.Add("var");
            r.B.Add("VarDecList");
            rule.Add(r);

            r = new Rule();
            r.A = "VarDecList";
            r.B.Add("TypeDef");
            r.B.Add("VarIdList");
            r.B.Add(";");
            r.B.Add("VarDecMore");
            rule.Add(r);

            r = new Rule();
            r.A = "VarDecMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "VarDecMore";
            r.B.Add("VarDecList");
            rule.Add(r);

            r = new Rule();
            r.A = "VarIdList";
            r.B.Add("ID");
            r.B.Add("VarIdMore");
            rule.Add(r);

            r = new Rule();
            r.A = "VarIdMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "VarIdMore";
            r.B.Add(",");
            r.B.Add("VarIdList");
            rule.Add(r);

            r = new Rule();
            r.A = "ProcDecpart";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "ProcDecpart";
            r.B.Add("ProcDec");
            rule.Add(r);

            r = new Rule();
            r.A = "ProcDec";
            r.B.Add("procedure");
            r.B.Add("ProcName");
            r.B.Add("(");
            r.B.Add("ParamList");
            r.B.Add(")");
            r.B.Add(";");
            r.B.Add("ProcDecPart");
            r.B.Add("ProcBody");
            r.B.Add("ProcDecMore");
            rule.Add(r);

            r = new Rule();
            r.A = "ProcDecMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "ProcDecMore";
            r.B.Add("ProcDec");
            rule.Add(r);

            r = new Rule();
            r.A = "ProcName";
            r.B.Add("ID");
            rule.Add(r);

            r = new Rule();
            r.A = "ParamList";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "ParamList";
            r.B.Add("ParamDecList");
            rule.Add(r);

            r = new Rule();
            r.A = "ParamDecList";
            r.B.Add("Param");
            r.B.Add("ParamMore");
            rule.Add(r);

            r = new Rule();
            r.A = "ParamMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "ParamMore";
            r.B.Add(";");
            r.B.Add("ParamDecList");
            rule.Add(r);

            r = new Rule();
            r.A = "Param";
            r.B.Add("TypeDef");
            r.B.Add("FormList");
            rule.Add(r);

            r = new Rule();
            r.A = "Param";
            r.B.Add("var");
            r.B.Add("TypeDef");
            r.B.Add("FormList");
            rule.Add(r);

            r = new Rule();
            r.A = "FormList";
            r.B.Add("ID");
            r.B.Add("FidMore");
            rule.Add(r);

            r = new Rule();
            r.A = "FidMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "FidMore";
            r.B.Add(",");
            r.B.Add("FormList");
            rule.Add(r);

            r = new Rule();
            r.A = "ProcDecPart";
            r.B.Add("DeclarePart");
            rule.Add(r);

            r = new Rule();
            r.A = "ProcBody";
            r.B.Add("ProgramBody");
            rule.Add(r);

            r = new Rule();
            r.A = "ProgramBody";
            r.B.Add("begin");
            r.B.Add("StmList");
            r.B.Add("end");
            rule.Add(r);

            r = new Rule();
            r.A = "StmList";
            r.B.Add("Stm");
            r.B.Add("StmMore");
            rule.Add(r);

            r = new Rule();
            r.A = "StmMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "StmMore";
            r.B.Add(";");
            r.B.Add("StmList");
            rule.Add(r);

            r = new Rule();
            r.A = "Stm";
            r.B.Add("ConditionalStm");
            rule.Add(r);

            r = new Rule();
            r.A = "Stm";
            r.B.Add("LoopStm");
            rule.Add(r);

            r = new Rule();
            r.A = "Stm";
            r.B.Add("InputStm");
            rule.Add(r);

            r = new Rule();
            r.A = "Stm";
            r.B.Add("OutputStm");
            rule.Add(r);

            r = new Rule();
            r.A = "Stm";
            r.B.Add("ReturnStm");
            rule.Add(r);

            r = new Rule();
            r.A = "Stm";
            r.B.Add("ID");
            r.B.Add("AssCall");
            rule.Add(r);

            r = new Rule();
            r.A = "AssCall";
            r.B.Add("AssignmentRest");
            rule.Add(r);

            r = new Rule();
            r.A = "AssCall";
            r.B.Add("CallStmRest");
            rule.Add(r);

            r = new Rule();
            r.A = "AssignmentRest";
            r.B.Add("VariMore");
            r.B.Add(":=");
            r.B.Add("Exp");
            rule.Add(r);

            r = new Rule();
            r.A = "ConditionalStm";
            r.B.Add("if");
            r.B.Add("RelExp");
            r.B.Add("then");
            r.B.Add("StmList");
            r.B.Add("else");
            r.B.Add("StmList");
            r.B.Add("fi");
            rule.Add(r);

            r = new Rule();
            r.A = "LoopStm";
            r.B.Add("while");
            r.B.Add("RelExp");
            r.B.Add("do");
            r.B.Add("StmList");
            r.B.Add("endwh");
            rule.Add(r);

            r = new Rule();
            r.A = "InputStm";
            r.B.Add("read");
            r.B.Add("(");
            r.B.Add("Invar");
            r.B.Add(")");
            rule.Add(r);

            r = new Rule();
            r.A = "Invar";
            r.B.Add("ID");
            rule.Add(r);

            r = new Rule();
            r.A = "OutputStm";
            r.B.Add("write");
            r.B.Add("(");
            r.B.Add("Exp");
            r.B.Add(")");
            rule.Add(r);

            r = new Rule();
            r.A = "ReturnStm";
            r.B.Add("return");
            r.B.Add("(");
            r.B.Add("Exp");
            r.B.Add(")");
            rule.Add(r);

            r = new Rule();
            r.A = "CallStmRest";
            r.B.Add("(");
            r.B.Add("ActParamList");
            r.B.Add(")");
            rule.Add(r);

            r = new Rule();
            r.A = "ActParamList";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "ActParamList";
            r.B.Add("Exp");
            r.B.Add("ActParamMore");
            rule.Add(r);

            r = new Rule();
            r.A = "ActParamMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "ActParamMore";
            r.B.Add(",");
            r.B.Add("ActParamList");
            rule.Add(r);

            r = new Rule();
            r.A = "RelExp";
            r.B.Add("Exp");
            r.B.Add("OtherRelE");
            rule.Add(r);

            r = new Rule();
            r.A = "OtherRelE";
            r.B.Add("CmpOp");
            r.B.Add("Exp");
            rule.Add(r);

            r = new Rule();
            r.A = "Exp";
            r.B.Add("Term");
            r.B.Add("OtherTerm");
            rule.Add(r);

            r = new Rule();
            r.A = "OtherTerm";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "OtherTerm";
            r.B.Add("AddOp");
            r.B.Add("Exp");
            rule.Add(r);

            r = new Rule();
            r.A = "Term";
            r.B.Add("Factor");
            r.B.Add("OtherFactor");
            rule.Add(r);

            r = new Rule();
            r.A = "OtherFactor";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "OtherFactor";
            r.B.Add("MulOp");
            r.B.Add("Term");
            rule.Add(r);

            r = new Rule();
            r.A = "Factor";
            r.B.Add("(");
            r.B.Add("Exp");
            r.B.Add(")");
            rule.Add(r);

            r = new Rule();
            r.A = "Factor";
            r.B.Add("INTC");
            rule.Add(r);

            r = new Rule();
            r.A = "Factor";
            r.B.Add("Variable");
            rule.Add(r);

            r = new Rule();
            r.A = "Variable";
            r.B.Add("ID");
            r.B.Add("VariMore");
            rule.Add(r);

            r = new Rule();
            r.A = "VariMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "VariMore";
            r.B.Add("[");
            r.B.Add("Exp");
            r.B.Add("]");
            rule.Add(r);

            r = new Rule();
            r.A = "VariMore";
            r.B.Add(".");
            r.B.Add("FieldVar");
            rule.Add(r);

            r = new Rule();
            r.A = "FieldVar";
            r.B.Add("ID");
            r.B.Add("FieldVarMore");
            rule.Add(r);

            r = new Rule();
            r.A = "FieldVarMore";
            r.B.Add("null");
            rule.Add(r);

            r = new Rule();
            r.A = "FieldVarMore";
            r.B.Add("[");
            r.B.Add("Exp");
            r.B.Add("]");
            rule.Add(r);

            r = new Rule();
            r.A = "CmpOp";
            r.B.Add("<");
            rule.Add(r);

            r = new Rule();
            r.A = "CmpOp";
            r.B.Add("=");
            rule.Add(r);

            r = new Rule();
            r.A = "AddOp";
            r.B.Add("+");
            rule.Add(r);

            r = new Rule();
            r.A = "AddOp";
            r.B.Add("-");
            rule.Add(r);

            r = new Rule();
            r.A = "MulOp";
            r.B.Add("*");
            rule.Add(r);

            r = new Rule();
            r.A = "MulOp";
            r.B.Add("/");
            rule.Add(r);
        }

    }
}
