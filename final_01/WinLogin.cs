using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace final_01
{
    public partial class WinLogin : Form
    {
        public WinLogin()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            skinEngine1.SkinFile = System.Environment.CurrentDirectory + "\\Skins\\Midsummer.ssk";//office2007.ssk
        }

        private void WinLogin_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //1. 获取数据
            //从TextBox中获取用户输入信息
            string userName = this.txtUserName.Text;
            string userPassword = this.txtPassword.Text;

            //2. 验证数据
            // 验证用户输入是否为空，若为空，提示用户信息
            if (userName.Equals("") || userPassword.Equals(""))
            {
                MessageBox.Show("用户名或密码不能为空！");
            }
            // 若不为空，验证用户名和密码是否与数据库匹配
            // 这里只做字符串对比验证
            else
            {
                //用户名和密码验证正确，提示成功，并执行跳转界面。
                if (userName.Equals("admin") && userPassword.Equals("admin"))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    MessageBox.Show("登录成功！");
                    //Application.Run(new Form1());
                    /**
                     * 待添加代码区域
                     * 实现界面跳转功能
                     * 
                     */

                }
                //用户名和密码验证错误，提示错误。
                else
                {
                    MessageBox.Show("用户名或密码错误！");

                }
            }

            //3. 处理数据
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe", "https://blog.csdn.net/qq_37832932");
        }
    }
}
