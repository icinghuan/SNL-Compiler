using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//递归下降法

namespace SNL_Compiler
{
    public class AttributeIR
    {
        public TypeIR idtype = new TypeIR();
        public string kind;
        public Var var;
        public Proc proc;
    }
    public class Var
    {
        public string access;
        public int level;
        public int off;
        public Boolean isParam;
    }
    public class Proc
    {
        public int level;
        public int mOff;
        public int nOff;
        public int procEntry;
        public int codeEntry;
    }
    public class TypeIR
    {
        public int size;
        public string kind;
        public Array array = null;
        public FieldChain body = null;
    }
    public class Array
    {
        public TypeIR indexTy = new TypeIR();
        public TypeIR elementTy = new TypeIR();
        public int low;
        public int up;
    }
    public class FieldChain
    {
        public string id;
        public int off;
        public TypeIR unitType = new TypeIR();
        public FieldChain next = null;
    }
    /*******************************************/
    public class TreeNode   /* 语法树结点的定义 */
    {
        public TreeNode[] child = new TreeNode[3];
        public TreeNode sibling = null;
        public int lineno;
        public string nodekind;
        public string kind;
        public int idnum;
        public string[] name = new string[10];
        public Attr attr = new Attr();
    }
    public class Attr
    {
        public ArrayAttr arrayAttr = null;  /* 只用到其中一个，用到时再分配内存 */
        public ProcAttr procAttr = null;
        public ExpAttr expAttr = null;
        public string type_name;
    }
    public class ArrayAttr
    {
        public int low;
        public int up;
        public string childtype;
    }
    public class ProcAttr
    {
        public string paramt;
    }
    public class ExpAttr
    {
        public string op;
        public int val;
        public string varkind;
        public string type;
    }


    /************************************************/
    public class TokenType       /****Token序列的定义*******/
    {
        public int lineshow;
        public string Lex;
        public string Sem;
    }


    /*public class stringTokenizer
    {

        public stringTokenizer(string str,string split)
        {
            string seps = @split;
            string[] tokens = System.Text.RegularExpressions.Regex.Split(str, seps);
        }
        public Boolean hasMoreTokens()
        {
            return true;
        }
        public string nextToken()
        {
            return "";
        }
    }*/

    /********************************************************************/
    /* 类  名 Recursion	                                            */
    /* 功  能 总程序的处理					            */
    /* 说  明 建立一个类，处理总程序                                    */
    /********************************************************************/
    public class Recursion
    {
        TokenType token = new TokenType();

        int lineno = 0;
        string temp_name;

        public Boolean Error = false;
        public string stree;
        public string serror;
        public TreeNode yufaTree;
        public static int tokenCount = 0;

        public Recursion()
        {
            yufaTree = Program();
            printTree(yufaTree, 0);
        }
        /********************************************************************/
        /********************************************************************/
        /********************************************************************/
        /* 函数名 Program					            */
        /* 功  能 总程序的处理函数					    */
        /* 产生式 < Program > ::= ProgramHead DeclarePart ProgramBody .     */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /*        语法树的根节点的第一个子节点指向程序头部分ProgramHead,    */
        /*        DeclaraPart为ProgramHead的兄弟节点,程序体部分ProgramBody  */
        /*        为DeclarePart的兄弟节点.                                  */
        /********************************************************************/
        TreeNode Program()
        {
            ReadNextToken();

            TreeNode root = newNode("ProcK");
            TreeNode t = ProgramHead();
            TreeNode q = DeclarePart();
            TreeNode s = ProgramBody();

            if (t != null)
                root.child[0] = t;
            else
                syntaxError("a program head is expected!");
            if (q != null)
                root.child[1] = q;
            if (s != null)
                root.child[2] = s;
            else syntaxError("a program body is expected!");

            match("DOT");
            //if (!(token.Lex.Equals("ENDFILE")))
                //syntaxError("Code ends before file\n");

            if (Error == true)
                return null;
            return root;
        }

        /**************************函数头部分********************************/
        /********************************************************************/
        /********************************************************************/
        /* 函数名 ProgramHead						    */
        /* 功  能 程序头的处理函数					    */
        /* 产生式 < ProgramHead > ::= PROGRAM  ProgramName                  */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ProgramHead()
        {
            TreeNode t = newNode("PheadK");
            match("PROGRAM");
            if (token.Lex.Equals("ID"))
                t.name[0] = token.Sem;
            match("ID");
            return t;
        }

        /**************************声明部分**********************************/
        /********************************************************************/
        /********************************************************************/
        /* 函数名 DeclarePart						    */
        /* 功  能 声明部分的处理					    */
        /* 产生式 < DeclarePart > ::= TypeDec  VarDec  ProcDec              */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode DeclarePart()
        {
            /*类型*/
            TreeNode typeP = newNode("TypeK");
            TreeNode tp1 = TypeDec();
            if (tp1 != null)
                typeP.child[0] = tp1;
            else
                typeP = null;

            /*变量*/
            TreeNode varP = newNode("VarK");
            TreeNode tp2 = VarDec();
            if (tp2 != null)
                varP.child[0] = tp2;
            else
                varP = null;

            /*函数*/
            TreeNode procP = ProcDec();
            if (procP == null) { }
            if (varP == null) { varP = procP; }
            if (typeP == null) { typeP = varP; }
            if (typeP != varP)
                typeP.sibling = varP;
            if (varP != procP)
                varP.sibling = procP;
            return typeP;
        }

        /**************************类型声明部分******************************/
        /********************************************************************/
        /* 函数名 TypeDec					            */
        /* 功  能 类型声明部分的处理    				    */
        /* 产生式 < TypeDec > ::= ε | TypeDeclaration                      */
        /* 说  明 根据文法产生式,调用相应的递归处理函数,生成语法树节点      */
        /********************************************************************/
        TreeNode TypeDec()
        {
            TreeNode t = null;
            if (token.Lex.Equals("TYPE"))
                t = TypeDeclaration();
            else if ((token.Lex.Equals("VAR")) || (token.Lex.Equals("PROCEDURE"))
                    || (token.Lex.Equals("BEGIN"))) { }
            else
                ReadNextToken();
            return t;
        }

        /********************************************************************/
        /* 函数名 TypeDeclaration					    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < TypeDeclaration > ::= TYPE  TypeDecList                 */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode TypeDeclaration()
        {
            match("TYPE");
            TreeNode t = TypeDecList();
            if (t == null)
                syntaxError("a type declaration is expected!");
            return t;
        }

        /********************************************************************/
        /* 函数名 TypeDecList		 				    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < TypeDecList > ::= TypeId = TypeName ; TypeDecMore       */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode TypeDecList()
        {
            TreeNode t = newNode("DecK");
            if (t != null)
            {
                TypeId(t);
                match("EQ");
                TypeName(t);
                match("SEMI");
                TreeNode p = TypeDecMore();
                if (p != null)
                    t.sibling = p;
            }
            return t;
        }

        /********************************************************************/
        /* 函数名 TypeDecMore		 				    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < TypeDecMore > ::=    ε | TypeDecList                   */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode TypeDecMore()
        {
            TreeNode t = null;
            if (token.Lex.Equals("ID"))
                t = TypeDecList();
            else if ((token.Lex.Equals("VAR")) || (token.Lex.Equals("PROCEDURE")) || (token.Lex.Equals("BEGIN"))) { }
            else
                ReadNextToken();
            return t;
        }

        /********************************************************************/
        /* 函数名 TypeId		 				    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < TypeId > ::= id                                         */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void TypeId(TreeNode t)
        {
            if ((token.Lex.Equals("ID")) && (t != null))
            {
                t.name[(t.idnum)] = token.Sem;
                t.idnum = t.idnum + 1;
            }
            match("ID");
        }

        /********************************************************************/
        /* 函数名 TypeName		 				    */
        /* 功  能 类型声明部分的处理				            */
        /* 产生式 < TypeName > ::= BaseType | StructureType | id            */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void TypeName(TreeNode t)
        {
            if (t != null)
            {
                if ((token.Lex.Equals("INTEGER")) || (token.Lex.Equals("CHAR")))
                    BaseType(t);
                else if ((token.Lex.Equals("ARRAY")) || (token.Lex.Equals("RECORD")))
                    StructureType(t);
                else if (token.Lex.Equals("ID"))
                {
                    t.kind = "IdK";
                    t.attr.type_name = token.Sem;
                    match("ID");
                }
                else
                    ReadNextToken();
            }
        }
        /********************************************************************/
        /* 函数名 BaseType		 				    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < BaseType > ::=  INTEGER | CHAR                          */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void BaseType(TreeNode t)
        {
            if (token.Lex.Equals("INTEGER"))
            {
                match("INTEGER");
                t.kind = "IntegerK";
            }
            else if (token.Lex.Equals("CHAR"))
            {
                match("CHAR");
                t.kind = "CharK";
            }
            else
                ReadNextToken();
        }

        /********************************************************************/
        /* 函数名 StructureType		 				    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < StructureType > ::=  ArrayType | RecType                */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void StructureType(TreeNode t)
        {
            if (token.Lex.Equals("ARRAY"))
            {
                ArrayType(t);
            }
            else if (token.Lex.Equals("RECORD"))
            {
                t.kind = "RecordK";
                RecType(t);
            }
            else
                ReadNextToken();
        }
        /********************************************************************/
        /* 函数名 ArrayType                                                 */
        /* 功  能 类型声明部分的处理函数			            */
        /* 产生式 < ArrayType > ::=  ARRAY [low..top] OF BaseType           */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void ArrayType(TreeNode t)
        {
            t.attr.arrayAttr = new ArrayAttr();
            match("ARRAY");
            match("LMIDPAREN");
            if (token.Lex.Equals("INTC"))
                t.attr.arrayAttr.low = int.Parse(token.Sem);
            match("INTC");
            match("UNDERANGE");
            if (token.Lex.Equals("INTC"))
                t.attr.arrayAttr.up = int.Parse(token.Sem);
            match("INTC");
            match("RMIDPAREN");
            match("OF");
            BaseType(t);
            t.attr.arrayAttr.childtype = t.kind;
            t.kind = "ArrayK";
        }

        /********************************************************************/
        /* 函数名 RecType		 				    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < RecType > ::=  RECORD FieldDecList END                  */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void RecType(TreeNode t)
        {
            TreeNode p = null;
            match("RECORD");
            p = FieldDecList();
            if (p != null)
                t.child[0] = p;
            else
                syntaxError("a record body is requested!");
            match("END");
        }
        /********************************************************************/
        /* 函数名 FieldDecList		 				    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < FieldDecList > ::=   BaseType IdList ; FieldDecMore     */
        /*                             | ArrayType IdList; FieldDecMore     */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode FieldDecList()
        {
            TreeNode t = newNode("DecK");
            TreeNode p = null;
            if (t != null)
            {
                if ((token.Lex.Equals("INTEGER")) || (token.Lex.Equals("CHAR")))
                {
                    BaseType(t);
                    IdList(t);
                    match("SEMI");
                    p = FieldDecMore();
                }
                else if (token.Lex.Equals("ARRAY"))
                {
                    ArrayType(t);
                    IdList(t);
                    match("SEMI");
                    p = FieldDecMore();
                }
                else
                {
                    ReadNextToken();
                    syntaxError("type name is expected");
                }
                t.sibling = p;
            }
            return t;
        }
        /********************************************************************/
        /* 函数名 FieldDecMore		 				    */
        /* 功  能 类型声明部分的处理函数			            */
        /* 产生式 < FieldDecMore > ::=  ε | FieldDecList                   */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode FieldDecMore()
        {
            TreeNode t = null;
            if (token.Lex.Equals("INTEGER") || token.Lex.Equals("CHAR") || token.Lex.Equals("ARRAY"))
                t = FieldDecList();
            else if (token.Lex.Equals("END")) { }
            else
                ReadNextToken();
            return t;
        }
        /********************************************************************/
        /* 函数名 IdList		 				    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < IdList > ::=  id  IdMore                                */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void IdList(TreeNode t)
        {
            if (token.Lex.Equals("ID"))
            {
                t.name[(t.idnum)] = token.Sem;
                t.idnum = t.idnum + 1;
                match("ID");
            }
            IdMore(t);
        }

        /********************************************************************/
        /* 函数名 IdMore		 				    */
        /* 功  能 类型声明部分的处理函数				    */
        /* 产生式 < IdMore > ::=  ε |  , IdList                            */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void IdMore(TreeNode t)
        {
            if (token.Lex.Equals("COMMA"))
            {
                match("COMMA");
                IdList(t);
            }
            else if (token.Lex.Equals("SEMI")) { }
            else
                ReadNextToken();
        }

        /**************************变量声明部分******************************/
        /********************************************************************/
        /* 函数名 VarDec		 				    */
        /* 功  能 变量声明部分的处理				            */
        /* 产生式 < VarDec > ::=  ε |  VarDeclaration                      */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode VarDec()
        {
            TreeNode t = null;
            if (token.Lex.Equals("VAR"))
                t = VarDeclaration();
            else if ((token.Lex.Equals("PROCEDURE")) || (token.Lex.Equals("BEGIN"))) { }
            else
                ReadNextToken();
            return t;
        }
        /********************************************************************/
        /* 函数名 VarDeclaration		 			    */
        /* 功  能 变量声明部分的处理函数				    */
        /* 产生式 < VarDeclaration > ::=  VAR  VarDecList                   */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode VarDeclaration()
        {
            match("VAR");
            TreeNode t = VarDecList();
            if (t == null)
                syntaxError("a var declaration is expected!");
            return t;
        }

        /********************************************************************/
        /* 函数名 VarDecList		 				    */
        /* 功  能 变量声明部分的处理函数				    */
        /* 产生式 < VarDecList > ::=  TypeName VarIdList; VarDecMore        */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode VarDecList()
        {
            TreeNode t = newNode("DecK");
            TreeNode p = null;
            if (t != null)
            {
                TypeName(t);
                VarIdList(t);
                match("SEMI");
                p = VarDecMore();
                t.sibling = p;
            }
            return t;
        }

        /********************************************************************/
        /* 函数名 VarDecMore		 				    */
        /* 功  能 变量声明部分的处理函数				    */
        /* 产生式 < VarDecMore > ::=  ε |  VarDecList                      */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode VarDecMore()
        {
            TreeNode t = null;
            if ((token.Lex.Equals("INTEGER")) || (token.Lex.Equals("CHAR")) || (token.Lex.Equals("ARRAY")) || (token.Lex.Equals("RECORD")) || (token.Lex.Equals("ID")))
                t = VarDecList();
            else if ((token.Lex.Equals("PROCEDURE")) || (token.Lex.Equals("BEGIN")))
            { }
            else
                ReadNextToken();
            return t;
        }

        /********************************************************************/
        /* 函数名 VarIdList		 				    */
        /* 功  能 变量声明部分的处理函数			            */
        /* 产生式 < VarIdList > ::=  id  VarIdMore                          */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void VarIdList(TreeNode t)
        {
            if (token.Lex.Equals("ID"))
            {
                t.name[(t.idnum)] = token.Sem;
                t.idnum = t.idnum + 1;
                match("ID");
            }
            else
            {
                syntaxError("a varid is expected here!");
                ReadNextToken();
            }
            VarIdMore(t);
        }

        /********************************************************************/
        /* 函数名 VarIdMore		 				    */
        /* 功  能 变量声明部分的处理函数				    */
        /* 产生式 < VarIdMore > ::=  ε |  , VarIdList                      */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void VarIdMore(TreeNode t)
        {
            if (token.Lex.Equals("COMMA"))
            {
                match("COMMA");
                VarIdList(t);
            }
            else if (token.Lex.Equals("SEMI")) { }
            else
                ReadNextToken();
        }
        /****************************过程声明部分****************************/
        /********************************************************************/
        /* 函数名 ProcDec		 		                    */
        /* 功  能 函数声明部分的处理					    */
        /* 产生式 < ProcDec > ::=  ε |  ProcDeclaration                    */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ProcDec()
        {
            TreeNode t = null;
            if (token.Lex.Equals("PROCEDURE"))
                t = ProcDeclaration();
            else if (token.Lex.Equals("BEGIN")) { }
            else
                ReadNextToken();
            return t;
        }
        /********************************************************************/
        /* 函数名 ProcDeclaration		 			    */
        /* 功  能 函数声明部分的处理函数				    */
        /* 产生式 < ProcDeclaration > ::=  PROCEDURE ProcName(ParamList);   */
        /*                                 ProcDecPart                      */
        /*                                 ProcBody                         */
        /*                                 ProcDecMore                      *
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ProcDeclaration()
        {
            TreeNode t = newNode("ProcDecK");
            match("PROCEDURE");
            if (token.Lex.Equals("ID"))
            {
                t.name[0] = token.Sem;
                t.idnum = t.idnum + 1;
                match("ID");
            }
            match("LPAREN");
            ParamList(t);
            match("RPAREN");
            match("SEMI");
            t.child[1] = ProcDecPart();
            t.child[2] = ProcBody();
            t.sibling = ProcDecMore();
            return t;
        }
        /********************************************************************/
        /* 函数名 ProcDecMore    				            */
        /* 功  能 更多函数声明中处理函数        	        	    */
        /* 产生式 < ProcDecMore > ::=  ε |  ProcDeclaration                */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ProcDecMore()
        {
            TreeNode t = null;
            if (token.Lex.Equals("PROCEDURE"))
                t = ProcDeclaration();
            else if (token.Lex.Equals("BEGIN")) { }
            else
                ReadNextToken();
            return t;
        }
        /********************************************************************/
        /* 函数名 ParamList		 				    */
        /* 功  能 函数声明中参数声明部分的处理函数	        	    */
        /* 产生式 < ParamList > ::=  ε |  ParamDecList                     */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void ParamList(TreeNode t)
        {
            TreeNode p = null;
            if ((token.Lex.Equals("INTEGER")) || (token.Lex.Equals("CHAR")) || (token.Lex.Equals("ARRAY")) || (token.Lex.Equals("RECORD")) || (token.Lex.Equals("ID")) || (token.Lex.Equals("VAR")))
            {
                p = ParamDecList();
                t.child[0] = p;
            }
            else if (token.Lex.Equals("RPAREN")) { }
            else
                ReadNextToken();
        }

        /********************************************************************/
        /* 函数名 ParamDecList		 			    	    */
        /* 功  能 函数声明中参数声明部分的处理函数	        	    */
        /* 产生式 < ParamDecList > ::=  Param  ParamMore                    */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ParamDecList()
        {
            TreeNode t = Param();
            TreeNode p = ParamMore();
            if (p != null)
                t.sibling = p;
            return t;
        }

        /********************************************************************/
        /* 函数名 ParamMore		 			    	    */
        /* 功  能 函数声明中参数声明部分的处理函数	        	    */
        /* 产生式 < ParamMore > ::=  ε | ; ParamDecList                    */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ParamMore()
        {
            TreeNode t = null;
            if (token.Lex.Equals("SEMI"))
            {
                match("SEMI");
                t = ParamDecList();
                if (t == null)
                    syntaxError("a param declaration is request!");
            }
            else if (token.Lex.Equals("RPAREN")) { }
            else
                ReadNextToken();
            return t;
        }
        /********************************************************************/
        /* 函数名 Param		 			    	            */
        /* 功  能 函数声明中参数声明部分的处理函数	        	    */
        /* 产生式 < Param > ::=  TypeName FormList | VAR TypeName FormList  */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode Param()
        {
            TreeNode t = newNode("DecK");
            if ((token.Lex.Equals("INTEGER")) || (token.Lex.Equals("CHAR")) || (token.Lex.Equals("ARRAY")) || (token.Lex.Equals("RECORD"))
               || (token.Lex.Equals("ID")))
            {
                t.attr.procAttr = new ProcAttr();
                t.attr.procAttr.paramt = "valparamType";
                TypeName(t);
                FormList(t);
            }
            else if (token.Lex.Equals("VAR"))
            {
                match("VAR");
                t.attr.procAttr = new ProcAttr();
                t.attr.procAttr.paramt = "varparamType";
                TypeName(t);
                FormList(t);
            }
            else
                ReadNextToken();
            return t;
        }

        /********************************************************************/
        /* 函数名 FormList		 			    	    */
        /* 功  能 函数声明中参数声明部分的处理函数	        	    */
        /* 产生式 < FormList > ::=  id  FidMore                             */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void FormList(TreeNode t)
        {
            if (token.Lex.Equals("ID"))
            {
                t.name[(t.idnum)] = token.Sem;
                t.idnum = t.idnum + 1;
                match("ID");
            }
            FidMore(t);
        }

        /********************************************************************/
        /* 函数名 FidMore		 			    	    */
        /* 功  能 函数声明中参数声明部分的处理函数	        	    */
        /* 产生式 < FidMore > ::=   ε |  , FormList                        */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        void FidMore(TreeNode t)
        {
            if (token.Lex.Equals("COMMA"))
            {
                match("COMMA");
                FormList(t);
            }
            else if ((token.Lex.Equals("SEMI")) || (token.Lex.Equals("RPAREN")))
            { }
            else
                ReadNextToken();
        }
        /********************************************************************/
        /* 函数名 ProcDecPart		 			  	    */
        /* 功  能 函数中的声明部分的处理函数	             	            */
        /* 产生式 < ProcDecPart > ::=  DeclarePart                          */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ProcDecPart()
        {
            TreeNode t = DeclarePart();
            return t;
        }

        /********************************************************************/
        /* 函数名 ProcBody		 			  	    */
        /* 功  能 函数体部分的处理函数	                    	            */
        /* 产生式 < ProcBody > ::=  ProgramBody                             */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ProcBody()
        {
            TreeNode t = ProgramBody();
            if (t == null)
                syntaxError("a program body is requested!");
            return t;
        }

        /****************************函数体部分******************************/
        /********************************************************************/
        /********************************************************************/
        /* 函数名 ProgramBody		 			  	    */
        /* 功  能 程序体部分的处理	                    	            */
        /* 产生式 < ProgramBody > ::=  BEGIN  StmList   END                 */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ProgramBody()
        {
            TreeNode t = newNode("StmLK");
            match("BEGIN");
            t.child[0] = StmList();
            match("END");
            return t;
        }

        /********************************************************************/
        /* 函数名 StmList		 			  	    */
        /* 功  能 语句部分的处理函数	                    	            */
        /* 产生式 < StmList > ::=  Stm    StmMore                           */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode StmList()
        {
            TreeNode t = Stm();
            TreeNode p = StmMore();
            if (t != null)
            {
                if (p != null)
                    t.sibling = p;
            }
            return t;
        }

        /********************************************************************/
        /* 函数名 StmMore		 			  	    */
        /* 功  能 语句部分的处理函数	                    	            */
        /* 产生式 < StmMore > ::=   ε |  ; StmList                         */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode StmMore()
        {
            TreeNode t = null;
            if ((token.Lex.Equals("ELSE")) || (token.Lex.Equals("FI")) || (token.Lex.Equals("END")) || (token.Lex.Equals("ENDWH"))) { }
            else if (token.Lex.Equals("SEMI"))
            {
                match("SEMI");
                t = StmList();
            }
            else
                ReadNextToken();
            return t;
        }
        /********************************************************************/
        /* 函数名 Stm   		 			  	    */
        /* 功  能 语句部分的处理函数	                    	            */
        /* 产生式 < Stm > ::=   ConditionalStm   {IF}                       */
        /*                    | LoopStm          {WHILE}                    */
        /*                    | InputStm         {READ}                     */
        /*                    | OutputStm        {WRITE}                    */
        /*                    | ReturnStm        {RETURN}                   */
        /*                    | id  AssCall      {id}                       */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode Stm()
        {
            TreeNode t = null;
            if (token.Lex.Equals("IF"))
                t = ConditionalStm();
            else if (token.Lex.Equals("WHILE"))
                t = LoopStm();
            else if (token.Lex.Equals("READ"))
                t = InputStm();
            else if (token.Lex.Equals("WRITE"))
                t = OutputStm();
            else if (token.Lex.Equals("RETURN"))
                t = ReturnStm();
            else if (token.Lex.Equals("ID"))
            {
                temp_name = token.Sem;
                match("ID");
                t = AssCall();
            }
            else
                ReadNextToken();
            return t;
        }

        /********************************************************************/
        /* 函数名 AssCall		 			  	    */
        /* 功  能 语句部分的处理函数	                    	            */
        /* 产生式 < AssCall > ::=   AssignmentRest   {:=,LMIDPAREN,DOT}     */
        /*                        | CallStmRest      {(}                    */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode AssCall()
        {
            TreeNode t = null;
            if ((token.Lex.Equals("ASSIGN")) || (token.Lex.Equals("LMIDPAREN")) || (token.Lex.Equals("DOT")))
                t = AssignmentRest();
            else if (token.Lex.Equals("LPAREN"))
                t = CallStmRest();
            else
                ReadNextToken();
            return t;
        }

        /********************************************************************/
        /* 函数名 AssignmentRest		 			    */
        /* 功  能 赋值语句部分的处理函数	                    	    */
        /* 产生式 < AssignmentRest > ::=  VariMore : = Exp                  */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode AssignmentRest()
        {
            TreeNode t = newStmtNode("AssignK");

            /* 赋值语句节点的第一个儿子节点记录赋值语句的左侧变量名，
            /* 第二个儿子结点记录赋值语句的右侧表达式*/

            /*处理第一个儿子结点，为变量表达式类型节点*/
            TreeNode c = newExpNode("VariK");
            c.name[0] = temp_name;
            c.idnum = c.idnum + 1;
            VariMore(c);
            t.child[0] = c;

            match("ASSIGN");

            /*处理第二个儿子节点*/
            t.child[1] = Exp();

            return t;
        }

        /********************************************************************/
        /* 函数名 ConditionalStm		 			    */
        /* 功  能 条件语句部分的处理函数	                    	    */
        /* 产生式 <ConditionalStm>::=IF RelExp THEN StmList ELSE StmList FI */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ConditionalStm()
        {
            TreeNode t = newStmtNode("IfK");
            match("IF");
            t.child[0] = Exp();
            match("THEN");
            if (t != null)
                t.child[1] = StmList();
            if (token.Lex.Equals("ELSE"))
            {
                match("ELSE");
                t.child[2] = StmList();
            }
            match("FI");
            return t;
        }

        /********************************************************************/
        /* 函数名 LoopStm          		 			    */
        /* 功  能 循环语句部分的处理函数	                    	    */
        /* 产生式 < LoopStm > ::=   WHILE RelExp DO StmList ENDWH           */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode LoopStm()
        {
            TreeNode t = newStmtNode("WhileK");
            match("WHILE");
            t.child[0] = Exp();
            match("DO");
            t.child[1] = StmList();
            match("ENDWH");
            return t;
        }

        /********************************************************************/
        /* 函数名 InputStm          		     	                    */
        /* 功  能 输入语句部分的处理函数	                    	    */
        /* 产生式 < InputStm > ::=  READ(id)                                */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode InputStm()
        {
            TreeNode t = newStmtNode("ReadK");
            match("READ");
            match("LPAREN");
            if (token.Lex.Equals("ID"))
            {
                t.name[0] = token.Sem;
                t.idnum = t.idnum + 1;
            }
            match("ID");
            match("RPAREN");
            return t;
        }

        /********************************************************************/
        /* 函数名 OutputStm          		     	                    */
        /* 功  能 输出语句部分的处理函数	                    	    */
        /* 产生式 < OutputStm > ::=   WRITE(Exp)                            */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode OutputStm()
        {
            TreeNode t = newStmtNode("WriteK");
            match("WRITE");
            match("LPAREN");
            t.child[0] = Exp();
            match("RPAREN");
            return t;
        }

        /********************************************************************/
        /* 函数名 ReturnStm          		     	                    */
        /* 功  能 返回语句部分的处理函数	                    	    */
        /* 产生式 < ReturnStm > ::=   RETURN(Exp)                           */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ReturnStm()
        {
            TreeNode t = newStmtNode("ReturnK");
            match("RETURN");
            return t;
        }

        /********************************************************************/
        /* 函数名 CallStmRest          		     	                    */
        /* 功  能 函数调用语句部分的处理函数	                  	    */
        /* 产生式 < CallStmRest > ::=  (ActParamList)                       */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode CallStmRest()
        {
            TreeNode t = newStmtNode("CallK");
            match("LPAREN");
            /*函数调用时，其子节点指向实参*/
            /*函数名的结点也用表达式类型结点*/
            TreeNode c = newExpNode("VariK");
            c.name[0] = temp_name;
            c.idnum = c.idnum + 1;
            t.child[0] = c;
            t.child[1] = ActParamList();
            match("RPAREN");
            return t;
        }

        /********************************************************************/
        /* 函数名 ActParamList          		   	            */
        /* 功  能 函数调用实参部分的处理函数	                	    */
        /* 产生式 < ActParamList > ::=     ε |  Exp ActParamMore           */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ActParamList()
        {
            TreeNode t = null;
            if (token.Lex.Equals("RPAREN")) { }
            else if ((token.Lex.Equals("ID")) || (token.Lex.Equals("INTC")))
            {
                t = Exp();
                if (t != null)
                    t.sibling = ActParamMore();
            }
            else
                ReadNextToken();
            return t;
        }

        /********************************************************************/
        /* 函数名 ActParamMore          		   	            */
        /* 功  能 函数调用实参部分的处理函数	                	    */
        /* 产生式 < ActParamMore > ::=     ε |  , ActParamList             */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点  */
        /********************************************************************/
        TreeNode ActParamMore()
        {
            TreeNode t = null;
            if (token.Lex.Equals("RPAREN")) { }
            else if (token.Lex.Equals("COMMA"))
            {
                match("COMMA");
                t = ActParamList();
            }
            else
                ReadNextToken();
            return t;
        }

        /*************************表达式部分********************************/
        /*******************************************************************/
        /* 函数名 Exp							   */
        /* 功  能 表达式处理函数					   */
        /* 产生式 Exp ::= simple_exp | 关系运算符  simple_exp              */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点 */
        /*******************************************************************/
        TreeNode Exp()
        {
            TreeNode t = simple_exp();

            /* 当前单词token为逻辑运算单词LT或者EQ */
            if ((token.Lex.Equals("LT")) || (token.Lex.Equals("EQ")))
            {
                TreeNode p = newExpNode("OpK");

                /* 将当前单词token(为EQ或者LT)赋给语法树节点p的运算符成员attr.op*/
                p.child[0] = t;
                p.attr.expAttr.op = token.Lex;
                t = p;

                /* 当前单词token与指定逻辑运算符单词(为EQ或者LT)匹配 */
                match(token.Lex);

                /* 语法树节点t非空,调用简单表达式处理函数simple_exp()	   
                   函数返回语法树节点给t的第二子节点成员child[1]  */
                if (t != null)
                    t.child[1] = simple_exp();
            }
            return t;
        }

        /*******************************************************************/
        /* 函数名 simple_exp						   */
        /* 功  能 表达式处理						   */
        /* 产生式 simple_exp ::=   term  |  加法运算符  term               */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点 */
        /*******************************************************************/
        TreeNode simple_exp()
        {
            TreeNode t = term();

            /* 当前单词token为加法运算符单词PLUS或MINUS */
            while ((token.Lex.Equals("PLUS")) || (token.Lex.Equals("MINUS")))
            {
                TreeNode p = newExpNode("OpK");
                p.child[0] = t;
                p.attr.expAttr.op = token.Lex;
                t = p;

                match(token.Lex);

                /* 调用元处理函数term(),函数返回语法树节点给t的第二子节点成员child[1] */
                t.child[1] = term();
            }
            return t;
        }

        /********************************************************************/
        /* 函数名 term						            */
        /* 功  能 项处理函数						    */
        /* 产生式 < 项 > ::=  factor | 乘法运算符  factor		    */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点 */
        /********************************************************************/
        TreeNode term()
        {
            TreeNode t = factor();

            /* 当前单词token为乘法运算符单词TIMES或OVER */
            while ((token.Lex.Equals("TIMES")) || (token.Lex.Equals("OVER")))
            {
                TreeNode p = newExpNode("OpK");
                p.child[0] = t;
                p.attr.expAttr.op = token.Lex;
                t = p;
                match(token.Lex);
                p.child[1] = factor();
            }
            return t;
        }

        /*********************************************************************/
        /* 函数名 factor						     */
        /* 功  能 因子处理函数						     */
        /* 产生式 factor ::= INTC | Variable | ( Exp )                       */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点 */
        /*********************************************************************/
        TreeNode factor()
        {
            TreeNode t = null;
            if (token.Lex.Equals("INTC"))
            {
                t = newExpNode("ConstK");

                /* 将当前单词名tokenstring转换为整数赋给t的数值成员attr.val */
                t.attr.expAttr.val = int.Parse(token.Sem);
                match("INTC");
            }
            else if (token.Lex.Equals("ID"))
                t = Variable();
            else if (token.Lex.Equals("LPAREN"))
            {
                match("LPAREN");
                t = Exp();
                match("RPAREN");
            }
            else
                ReadNextToken();
            return t;
        }


        /********************************************************************/
        /* 函数名 Variable						    */
        /* 功  能 变量处理函数						    */
        /* 产生式 Variable   ::=   id VariMore                   	    */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点 */
        /********************************************************************/
        TreeNode Variable()
        {
            TreeNode t = newExpNode("VariK");
            if (token.Lex.Equals("ID"))
            {
                t.name[0] = token.Sem;
                t.idnum = t.idnum + 1;
            }
            match("ID");
            VariMore(t);
            return t;
        }

        /********************************************************************/
        /* 函数名 VariMore						    */
        /* 功  能 变量处理						    */
        /* 产生式 VariMore   ::=  ε                             	    */
        /*                       | [Exp]            {[}                     */
        /*                       | . FieldVar       {DOT}                   */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点 */
        /********************************************************************/
        void VariMore(TreeNode t)
        {
            if ((token.Lex.Equals("EQ")) || (token.Lex.Equals("LT")) || (token.Lex.Equals("PLUS")) || (token.Lex.Equals("MINUS")) || (token.Lex.Equals("RPAREN")) || (token.Lex.Equals("RMIDPAREN")) || (token.Lex.Equals("SEMI")) || (token.Lex.Equals("COMMA")) ||
               (token.Lex.Equals("THEN")) || (token.Lex.Equals("ELSE")) || (token.Lex.Equals("FI")) || (token.Lex.Equals("DO")) || (token.Lex.Equals("ENDWH")) || (token.Lex.Equals("END")) || (token.Lex.Equals("ASSIGN")) || (token.Lex.Equals("TIMES")) || (token.Lex.Equals("OVER"))) { }
            else if (token.Lex.Equals("LMIDPAREN"))
            {
                match("LMIDPAREN");
                t.child[0] = Exp();
                t.attr.expAttr.varkind = "ArrayMembV";
                match("RMIDPAREN");
            }
            else if (token.Lex.Equals("DOT"))
            {
                match("DOT");
                t.child[0] = FieldVar();
                t.attr.expAttr.varkind = "FieldMembV";
            }
            else
                ReadNextToken();
        }
        /********************************************************************/
        /* 函数名 FieldVar						    */
        /* 功  能 变量处理函数				                    */
        /* 产生式 FieldVar   ::=  id  FieldVarMore                          */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点 */
        /********************************************************************/
        TreeNode FieldVar()
        {
            TreeNode t = newExpNode("VariK");
            if (token.Lex.Equals("ID"))
            {
                t.name[0] = token.Sem;
                t.idnum = t.idnum + 1;
            }
            match("ID");
            FieldVarMore(t);
            return t;
        }

        /********************************************************************/
        /* 函数名 FieldVarMore  			                    */
        /* 功  能 变量处理函数                                              */
        /* 产生式 FieldVarMore   ::=  ε| [Exp]            {[}              */
        /* 说  明 函数根据文法产生式,调用相应的递归处理函数,生成语法树节点 */
        /********************************************************************/
        void FieldVarMore(TreeNode t)
        {
            if ((token.Lex.Equals("ASSIGN")) || (token.Lex.Equals("TIMES")) || (token.Lex.Equals("EQ")) || (token.Lex.Equals("LT")) || (token.Lex.Equals("PLUS")) || (token.Lex.Equals("MINUS")) || (token.Lex.Equals("OVER")) || (token.Lex.Equals("RPAREN")) || (token.Lex.Equals("SEMI")) || (token.Lex.Equals("COMMA")) || (token.Lex.Equals("THEN")) || (token.Lex.Equals("ELSE")) || (token.Lex.Equals("FI")) || (token.Lex.Equals("DO")) ||
               (token.Lex.Equals("ENDWH")) || (token.Lex.Equals("END")))
            { }
            else if (token.Lex.Equals("LMIDPAREN"))
            {
                match("LMIDPAREN");
                t.child[0] = Exp();
                t.child[0].attr.expAttr.varkind = "ArrayMembV";
                match("RMIDPAREN");
            }
            else
                ReadNextToken();
        }

        /********************************************************************/
        /********************************************************************/
        /* 函数名 match							    */
        /* 功  能 终极符匹配处理函数				            */
        /* 说  明 函数参数expected给定期望单词符号与当前单词符号token相匹配 */
        /*        如果不匹配,则报非期望单词语法错误			    */
        /********************************************************************/
        void match(string expected)
        {
            if (token.Lex.Equals(expected))
                ReadNextToken();
            else
            {
                syntaxError("not match error ");
                ReadNextToken();
            }
        }

        /************************************************************/
        /* 函数名 syntaxError                                       */
        /* 功  能 语法错误处理函数		                    */
        /* 说  明 将函数参数message指定的错误信息输出               */
        /************************************************************/
        void syntaxError(string s)     /*向错误信息.txt中写入字符串*/
        {
            serror = serror + "\n>>> ERROR :" + "Syntax error at " + Convert.ToString(token.lineshow) + ": " + s;

            /* 设置错误追踪标志Error为TRUE,防止错误进一步传递 */
            Error = true;
        }

        /********************************************************************/
        /* 函数名 ReadNextToken                                             */
        /* 功  能 从Token序列中取出一个Token				    */
        /* 说  明 从文件中存的Token序列中依次取一个单词，作为当前单词       */
        /********************************************************************/

        void ReadNextToken()
        {
            if (tokenCount < Data.token.Count)
            {
                token.lineshow = Data.token[tokenCount].line;
                lineno = token.lineshow;
                token.Sem = Data.token[tokenCount].sem;
                switch (Data.token[tokenCount].lex)
                {
                    case 1:
                        if (token.Sem == ":=")
                        {
                            token.Lex = "ASSIGN";
                            break;
                        }
                        if (token.Sem == "..")
                        {
                            token.Lex = "UNDERANGE";
                            break;
                        }
                        switch (token.Sem[0])
                        {
                            case '.':
                                token.Lex = "DOT";
                                break;
                            /*
						 * 当前字符c为"=",当前识别单词返回值token设置为 等号单词EQ
						 */
                            case '=':
                                token.Lex = "EQ";
                                break;

                            /*
                             * 当前字符c为"<",当前识别单词返回值token设置为 小于单词LT
                             */
                            case '<':
                                token.Lex = "LT";
                                break;

                            /*
                             * 当前字符c为"+",当前识别单词返回值token设置为 加号单词PLUS
                             */
                            case '+':
                                token.Lex = "PLUS";
                                break;

                            /*
                             * 当前字符c为"-",当前识别单词返回值token设置为 减号单词MINUS
                             */
                            case '-':
                                token.Lex = "MINUS";
                                break;

                            /*
                             * 当前字符c为"*",当前识别单词返回值token设置为 乘号单词TIMES
                             */
                            case '*':
                                token.Lex = "TIMES";
                                break;

                            /*
                             * 当前字符c为"/",当前识别单词返回值token设置为 除号单词OVER
                             */
                            case '/':
                                token.Lex = "OVER";
                                break;

                            /*
                             * 当前字符c为"(",当前识别单词返回值token设置为 左括号单词LPAREN
                             */
                            case '(':
                                token.Lex = "LPAREN";
                                break;

                            /*
                             * 当前字符c为")",当前识别单词返回值token设置为 右括号单词RPAREN
                             */
                            case ')':
                                token.Lex = "RPAREN";
                                break;

                            /*
                             * 当前字符c为";",当前识别单词返回值token设置为 分号单词SEMI
                             */
                            case ';':
                                token.Lex = "SEMI";
                                break;
                            /*
                             * 当前字符c为",",当前识别单词返回值token设置为 逗号单词COMMA
                             */
                            case ',':
                                token.Lex = "COMMA";
                                break;
                            /*
                             * 当前字符c为"[",当前识别单词返回值token设置为 左中括号单词LMIDPAREN
                             */
                            case '[':
                                token.Lex = "LMIDPAREN";
                                break;

                            /*
                             * 当前字符c为"]",当前识别单词返回值token设置为 右中括号单词RMIDPAREN
                             */
                            case ']':
                                token.Lex = "RMIDPAREN";
                                break;

                            /*
                             * 当前字符c为其它字符,当前识别单词返回值token 设置为错误单词ERROR
                             */
                            default:
                                token.Lex = "ERROR";
                                Error = true;
                                break;
                        }
                        break;
                    case 2:
                        token.Lex = token.Sem.ToUpper();
                        break;
                    case 3:
                        token.Lex = "ID";
                        break;
                    case 4:
                        token.Lex = "INTC";
                        break;
                    default:
                        token.Lex = "";
                        break;
                }
                tokenCount++;
            }
        }

        /********************************************************
         *********以下是创建语法树所用的各类节点的申请***********
         ********************************************************/
        /********************************************************/
        /* 函数名 newNode				        */
        /* 功  能 创建语法树节点函数			        */
        /* 说  明 该函数为语法树创建一个新的结点      	        */
        /*        并将语法树节点成员赋初值。 s为ProcK, PheadK,  */
        /*        DecK, TypeK, VarK, ProcDecK, StmLK	        */
        /********************************************************/
        TreeNode newNode(string s)
        {
            TreeNode t = new TreeNode();
            t.nodekind = s;
            t.lineno = lineno;
            return t;
        }
        /********************************************************/
        /* 函数名 newStmtNode					*/
        /* 功  能 创建语句类型语法树节点函数			*/
        /* 说  明 该函数为语法树创建一个新的语句类型结点	*/
        /*        并将语法树节点成员初始化			*/
        /********************************************************/
        TreeNode newStmtNode(string s)
        {
            TreeNode t = new TreeNode();
            t.nodekind = "StmtK";
            t.lineno = lineno;
            t.kind = s;
            return t;
        }
        /********************************************************/
        /* 函数名 newExpNode					*/
        /* 功  能 表达式类型语法树节点创建函数			*/
        /* 说  明 该函数为语法树创建一个新的表达式类型结点	*/
        /*        并将语法树节点的成员赋初值			*/
        /********************************************************/
        TreeNode newExpNode(string s)
        {
            TreeNode t = new TreeNode();
            t.nodekind = "ExpK";
            t.kind = s;
            t.lineno = lineno;
            t.attr.expAttr = new ExpAttr();
            t.attr.expAttr.varkind = "IdV";
            t.attr.expAttr.type = "Void";
            return t;
        }
        /********************************************************************/
        /* 函数组                    					    */
        /* 功  能 将语法树写入文件				            */
        /* 说  明 3个函数将不同的信息写入语法树                             */
        /********************************************************************/
        void writeStr(string s)                   /*写入字符串*/
        {
            stree = stree + s;
        }

        void writeSpace()                            /*写空格*/
        {
            stree = stree + "  ";
        }

        void writeTab(int x)              /*写换行符和4个空格*/
        {
            stree = stree + "\n";
            while (x != 0)
            { stree = stree + "\t"; x--; }
        }
        /******************************************************/
        /* 函数名 printTree                                   */
        /* 功  能 把语法树输出，显示在文件中                  */
        /* 说  明                                             */
        /******************************************************/
        void printTree(TreeNode t, int l)
        {
            TreeNode tree = t;
            while (tree != null)
            {
                if (tree.nodekind.Equals("ProcK"))
                {
                    stree = "ProcK";
                }
                else if (tree.nodekind.Equals("PheadK"))
                {
                    writeTab(1);
                    writeStr("PheadK");
                    writeSpace();
                    writeStr(tree.name[0]);
                }
                else if (tree.nodekind.Equals("DecK"))
                {
                    writeTab(l);
                    writeStr("DecK");
                    writeSpace();
                    if (tree.attr.procAttr != null)
                    {
                        if (tree.attr.procAttr.paramt.Equals("varparamType"))
                            writeStr("Var param:");
                        else if (tree.attr.procAttr.paramt.Equals("valparamType"))
                            writeStr("Value param:");
                    }
                    if (tree.kind.Equals("ArrayK"))
                    {
                        writeStr("ArrayK");
                        writeSpace();
                        writeStr(Convert.ToString(tree.attr.arrayAttr.low));
                        writeSpace();
                        writeStr(Convert.ToString(tree.attr.arrayAttr.up));
                        writeSpace();
                        if (tree.attr.arrayAttr.childtype.Equals("CharK"))
                            writeStr("CharK");
                        else if (tree.attr.arrayAttr.childtype.Equals("IntegerK"))
                            writeStr("IntegerK");
                    }
                    else if (tree.kind.Equals("CharK"))
                        writeStr("CharK");
                    else if (tree.kind.Equals("IntegerK"))
                        writeStr("IntegerK");
                    else if (tree.kind.Equals("RecordK"))
                        writeStr("RecordK");
                    else if (tree.kind.Equals("IdK"))
                    {
                        writeStr("IdK");
                        writeStr(tree.attr.type_name);
                    }
                    else
                        syntaxError("error1!");
                    if (tree.idnum != 0)
                        for (int i = 0; i < (tree.idnum); i++)
                        { writeSpace(); writeStr(tree.name[i]); }
                    else
                        syntaxError("wrong!no var!");
                }
                else if (tree.nodekind.Equals("TypeK"))
                {
                    writeTab(l);
                    writeStr("TypeK");
                }
                else if (tree.nodekind.Equals("VarK"))
                {
                    writeTab(l);
                    writeStr("VarK");
                }
                else if (tree.nodekind.Equals("ProcDecK"))
                {
                    writeTab(l);
                    writeStr("ProcDecK");
                    writeSpace();
                    writeStr(tree.name[0]);
                }
                else if (tree.nodekind.Equals("StmLK"))
                {
                    writeTab(l);
                    writeStr("StmLK");
                }
                else if (tree.nodekind.Equals("StmtK"))
                {
                    writeTab(l);
                    writeStr("StmtK");
                    writeSpace();
                    if (tree.kind.Equals("IfK"))
                        writeStr("If");
                    else if (tree.kind.Equals("WhileK"))
                        writeStr("While");
                    else if (tree.kind.Equals("AssignK"))
                        writeStr("Assign");
                    else if (tree.kind.Equals("ReadK"))
                    {
                        writeStr("Read");
                        writeSpace();
                        writeStr(tree.name[0]);
                    }
                    else if (tree.kind.Equals("WriteK"))
                        writeStr("Write");
                    else if (tree.kind.Equals("CallK"))
                        writeStr("Call");
                    else if (tree.kind.Equals("ReturnK"))
                        writeStr("Return");
                    else
                        syntaxError("error2!");
                }
                else if (tree.nodekind.Equals("ExpK"))
                {
                    writeTab(l);
                    writeStr("ExpK");
                    if (tree.kind.Equals("OpK"))
                    {
                        writeSpace();
                        writeStr("Op");
                        writeSpace();
                        if (tree.attr.expAttr.op.Equals("EQ"))
                            writeStr("=");
                        else if (tree.attr.expAttr.op.Equals("LT"))
                            writeStr("<");
                        else if (tree.attr.expAttr.op.Equals("PLUS"))
                            writeStr("+");
                        else if (tree.attr.expAttr.op.Equals("MINUS"))
                            writeStr("-");
                        else if (tree.attr.expAttr.op.Equals("TIMES"))
                            writeStr("*");
                        else if (tree.attr.expAttr.op.Equals("OVER"))
                            writeStr("/");
                        else
                            syntaxError("error3!");
                    }
                    else if (tree.kind.Equals("ConstK"))
                    {
                        writeSpace();
                        writeStr("Const");
                        writeSpace();
                        writeStr(Convert.ToString(tree.attr.expAttr.val));
                    }
                    else if (tree.kind.Equals("VariK"))
                    {
                        writeSpace();
                        writeStr("Vari");
                        writeSpace();
                        if (tree.attr.expAttr.varkind.Equals("IdV"))
                        {
                            writeStr("Id");
                            writeSpace();
                            writeStr(tree.name[0]);
                        }
                        else if (tree.attr.expAttr.varkind.Equals("FieldMembV"))
                        {
                            writeStr("FieldMember");
                            writeSpace();
                            writeStr(tree.name[0]);
                        }
                        else if (tree.attr.expAttr.varkind.Equals("ArrayMembV"))
                        {
                            writeStr("ArrayMember");
                            writeSpace();
                            writeStr(tree.name[0]);
                        }
                        else
                            syntaxError("var type error!");
                    }
                    else
                        syntaxError("error4!");
                }
                else
                    syntaxError("error5!");

                /* 对语法树结点tree的各子结点递归调用printTree过程 */
                for (int i = 0; i < 3; i++)
                    printTree(tree.child[i], l + 1);
                /* 对语法树结点tree的各兄弟结点递归调用printTree过程 */
                tree = tree.sibling;
            }
        }

    }
}
