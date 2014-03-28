using System;
using System.Windows;


/// <summary>
/// RTF文档书写器
/// </summary>
/// <remarks>
/// 本书写器对生成RTF文档提供了基础的支持
/// 编制 袁永福 http://www.xdesigner.cn
/// </remarks>
public class RTFWriter : System.IDisposable
{

    #region 测试代码 ******************************************************

    [System.STAThread]
    static void Main()
    {
        DateTime dateTime = System.DateTime.Now;


        long l1 = 1;
        long l2 = 2;

        float st = (float)l1 / l2;



        string directory = st.ToString("p");//this.Request.PhysicalApplicationPath + 
        Console.WriteLine(directory);
        Console.ReadLine();
        


        //TestWriteFile();
        //TestClipboard();
    }

    /// <summary>
    /// 测试生成RTF文件
    /// 执行这个函数后可以使用 MS Word 打开文件 c:\a.rtf 
    /// </summary>
    internal static void TestWriteFile()
    {
        RTFWriter w = new RTFWriter("c:\\a.rtf");
        TestBuildRTF(w);
        w.Close();
        System.Windows.Forms.MessageBox.Show("好了,你可以打开文件 c:\\a.rtf 了.");
    }

    /// <summary>
    /// 测试生成RTF文档并设置到系统剪切板中
    /// 执行这个函数后就可以在 MS Word中使用粘贴操作来显示程序生成的文档了
    /// </summary>
    internal static void TestClipboard()
    {
        System.IO.StringWriter myStr = new System.IO.StringWriter();
        RTFWriter w = new RTFWriter(myStr);
        TestBuildRTF(w);
        w.Close();
        System.Windows.Forms.DataObject data = new System.Windows.Forms.DataObject();
        data.SetData(System.Windows.Forms.DataFormats.Rtf, myStr.ToString());
        System.Windows.Forms.Clipboard.SetDataObject(data, true);
        System.Windows.Forms.MessageBox.Show("好了,你可以在MS Word 中粘贴文本了.");
    }

    /// <summary>
    /// 测试生成RTF文档
    /// </summary>
    /// <param name="w">RTF文档书写器</param>
    private static void TestBuildRTF(RTFWriter w)
    {
        w.Encoding = System.Text.Encoding.GetEncoding(936);
        // 输出文件头
        w.WriteStartGroup();
        w.WriteKeyword("rtf1");
        w.WriteKeyword("ansi");
        w.WriteKeyword("ansicpg" + w.Encoding.CodePage);
        // 输出字体表
        w.WriteStartGroup();
        w.WriteKeyword("fonttbl");
        w.WriteStartGroup();
        w.WriteKeyword("f0");
        w.WriteText("隶书;");
        w.WriteEndGroup();
        w.WriteStartGroup();
        w.WriteKeyword("f1");
        w.WriteText("宋体;");
        w.WriteEndGroup();
        w.WriteEndGroup();
        // 输出颜色表
        w.WriteStartGroup();
        w.WriteKeyword("colortbl");
        w.WriteText(";");
        w.WriteKeyword("red0");
        w.WriteKeyword("green0");
        w.WriteKeyword("blue255");
        w.WriteText(";");
        w.WriteEndGroup();
        // 输出正文
        w.WriteKeyword("qc");    // 设置居中对齐
        w.WriteKeyword("f0");    // 设置字体
        w.WriteKeyword("fs30");    // 字体大小
        w.WriteText("这是第一段文本 ");
        w.WriteKeyword("cf1");    // 设置颜色
        w.WriteText("隶书 ");
        w.WriteKeyword("cf0");    // 设置为默认颜色
        w.WriteKeyword("f1");    // 设置字体
        w.WriteText("居中对齐 ABC12345");
        w.WriteKeyword("par");    // 开始新的段落
        w.WriteKeyword("pard");    // 清除居中对齐
        w.WriteKeyword("f1");    // 设置字体
        w.WriteKeyword("fs20");    // 字体大小
        w.WriteKeyword("cf1");
        w.WriteText("这是第二段文本 宋体 左对齐 ABC12345");
        // 结束输出
        w.WriteEndGroup();
    }

    #endregion

    /// <summary>
    /// 初始化对象
    /// </summary>
    /// <param name="w">文本书写器</param>
    public RTFWriter(System.IO.TextWriter w)
    {
        myWriter = w;
    }

    /// <summary>
    /// 初始化对象
    /// </summary>
    /// <param name="strFileName">文件名</param>
    public RTFWriter(string strFileName)
    {
        myWriter = new System.IO.StreamWriter(
            strFileName,
            false,
            System.Text.Encoding.ASCII);
    }

    private System.Text.Encoding myEncoding = System.Text.Encoding.GetEncoding(936);
    /// <summary>
    /// 字符编码格式
    /// </summary>
    public System.Text.Encoding Encoding
    {
        get { return myEncoding; }
        set { myEncoding = value; }
    }

    /// <summary>
    /// 内置的文本书写器
    /// </summary>
    private System.IO.TextWriter myWriter = null;

    private bool bolIndent = false;
    /// <summary>
    /// 是否使用缩进
    /// </summary>
    /// <remarks>
    /// RTF文档内部不能随便缩进，提供此选项只是用于生成便于阅读的RTF文档，便于程序的调试，
    /// 在开发调试中可以设置该属性为true,方便开发者能直接查看生成的RTF文档，但在生成最终运行的
    /// 程序时应当设置该属性为 false .
    /// </remarks>
    public bool Indent
    {
        get { return bolIndent; }
        set { bolIndent = value; }
    }

    private string strIndentString = "   ";
    /// <summary>
    /// 缩进字符串
    /// </summary>
    public string IndentString
    {
        get { return strIndentString; }
        set { strIndentString = value; }
    }
    /// <summary>
    /// 当前缩进层次
    /// </summary>
    private int intGroupLevel = 0;

    /// <summary>
    /// 关闭对象
    /// </summary>
    public void Close()
    {
        if (this.intGroupLevel > 0)
            throw new System.Exception("还有组未写完");
        if (myWriter != null)
        {
            myWriter.Close();
            myWriter = null;
        }
    }

    /// <summary>
    /// 输出一个组
    /// </summary>
    /// <param name="KeyWord">关键字</param>
    public void WriteGroup(string KeyWord)
    {
        this.WriteStartGroup();
        this.WriteKeyword(KeyWord);
        this.WriteEndGroup();
    }

    /// <summary>
    /// 开始输出组
    /// </summary>
    public void WriteStartGroup()
    {
        if (bolIndent)
        {
            InnerWriteNewLine();
            myWriter.Write("{");
        }
        else
            myWriter.Write("{");
        intGroupLevel++;
    }

    /// <summary>
    /// 结束输出组
    /// </summary>
    public void WriteEndGroup()
    {
        intGroupLevel--;
        if (intGroupLevel < 0)
            throw new System.Exception("组不匹配");
        if (bolIndent)
        {
            InnerWriteNewLine();
            InnerWrite("}");
        }
        else
            InnerWrite("}");
    }

    /// <summary>
    /// 输出原始文本
    /// </summary>
    /// <param name="txt">文本值</param>
    public void WriteRaw(string txt)
    {
        if (txt != null && txt.Length > 0)
        {
            InnerWrite(txt);
        }
    }
    /// <summary>
    /// 输出关键字
    /// </summary>
    /// <param name="Keyword">关键字值</param>
    public void WriteKeyword(string Keyword)
    {
        WriteKeyword(Keyword, false);
    }
    /// <summary>
    /// 输出关键字
    /// </summary>
    /// <param name="Keyword">关键字值</param>
    /// <param name="Ext">是否是扩展关键字</param>
    public void WriteKeyword(string Keyword, bool Ext)
    {
        if (Keyword == null || Keyword.Length == 0)
            throw new System.ArgumentNullException("值不得为空");
        if (bolIndent == false && (Keyword == "par" || Keyword == "pard"))
        {
            // par 或 pard 前可以输出空白行，不影响RTF文档显示
            InnerWrite(System.Environment.NewLine);
        }
        if (this.bolIndent)
        {
            if (Keyword == "par" || Keyword == "pard")
            {
                this.InnerWriteNewLine();
            }
        }
        if (Ext)
            InnerWrite("\\*\\");
        else
            InnerWrite("\\");
        InnerWrite(Keyword);
    }

    /// <summary>
    /// 内容文本编码格式
    /// </summary>
    private System.Text.Encoding Unicode = System.Text.Encoding.Unicode;
    /// <summary>
    /// 输出纯文本
    /// </summary>
    /// <param name="Text">文本值</param>
    public void WriteText(string Text)
    {
        if (Text == null || Text.Length == 0)
            return;

        InnerWrite(' ');

        for (int iCount = 0; iCount < Text.Length; iCount++)
        {
            char c = Text[iCount];
            if (c == '\t')
            {
                this.WriteKeyword("tab");
                InnerWrite(' ');
            }
            else if (c < 256)
            {
                if (c > 32 && c < 127)
                {
                    // 出现特殊字符，需要斜线转义
                    if (c == '\\' || c == '{' || c == '}')
                        InnerWrite('\\');
                    InnerWrite(c);
                }
                else
                {
                    InnerWrite("\\\'");
                    WriteByte((byte)c);
                }
            }
            else
            {
                byte[] bs = myEncoding.GetBytes(c.ToString());
                for (int iCount2 = 0; iCount2 < bs.Length; iCount2++)
                {
                    InnerWrite("\\\'");
                    WriteByte(bs[iCount2]);
                }
            }
        }//for( int iCount = 0 ; iCount < Text.Length ; iCount ++ )
    }

    /// <summary>
    /// 当前位置
    /// </summary>
    private int intPosition = 0;
    /// <summary>
    /// 当前行的位置
    /// </summary>
    private int intLineHead = 0;

    /// <summary>
    /// 16进制字符组
    /// </summary>
    private const string Hexs = "0123456789abcdef";

    /// <summary>
    /// 输出字节数组
    /// </summary>
    /// <param name="bs">字节数组</param>
    public void WriteBytes(byte[] bs)
    {
        if (bs == null || bs.Length == 0)
            return;
        WriteRaw(" ");
        for (int iCount = 0; iCount < bs.Length; iCount++)
        {
            if ((iCount % 32) == 0)
            {
                this.WriteRaw(System.Environment.NewLine);
                this.WriteIndent();
            }
            else if ((iCount % 8) == 0)
            {
                this.WriteRaw(" ");
            }
            byte b = bs[iCount];
            int h = (b & 0xf0) >> 4;
            int l = b & 0xf;
            myWriter.Write(Hexs[h]);
            myWriter.Write(Hexs[l]);
            intPosition += 2;
        }
    }

    /// <summary>
    /// 输出一个字节数据
    /// </summary>
    /// <param name="b">字节数据</param>
    public void WriteByte(byte b)
    {
        int h = (b & 0xf0) >> 4;
        int l = b & 0xf;
        myWriter.Write(Hexs[h]);
        myWriter.Write(Hexs[l]);
        intPosition += 2;
        //FixIndent();
    }

    #region 内部成员 ******************************************************

    private void InnerWrite(char c)
    {
        intPosition++;
        myWriter.Write(c);
    }
    private void InnerWrite(string txt)
    {
        intPosition += txt.Length;
        myWriter.Write(txt);
    }

    private void FixIndent()
    {
        if (this.bolIndent)
        {
            if (intPosition - intLineHead > 100)
                InnerWriteNewLine();
        }
    }

    private void InnerWriteNewLine()
    {
        if (this.bolIndent)
        {
            if (intPosition > 0)
            {
                InnerWrite(System.Environment.NewLine);
                intLineHead = intPosition;
                WriteIndent();
            }
        }
    }

    private void WriteIndent()
    {
        if (bolIndent)
        {
            for (int iCount = 0; iCount < intGroupLevel; iCount++)
            {
                InnerWrite(this.strIndentString);
            }
        }
    }

    #endregion

    /// <summary>
    /// 销毁对象
    /// </summary>
    public void Dispose()
    {
        this.Close();
    }
}
