
using DynamicDataChecking.Common;
using System;
using System.Windows.Forms;

namespace DynamicDataChecking.CommonClass
{
    public class OpaqueCommand
    {
        private MyOpaqueLayer.MyOpaqueLayer m_OpaqueLayer = null;//��͸���ɰ��

        public OpaqueCommand(int alpha, bool isShowLoadingImage)
        {
            if (m_OpaqueLayer == null)
            {
                m_OpaqueLayer = new MyOpaqueLayer.MyOpaqueLayer(alpha, isShowLoadingImage);
            }
        }

        /// <summary>
        /// ��ʾ���ֲ�
        /// </summary>
        /// <param name="control">�ؼ�</param>
        /// <param name="alpha">͸����</param>
        /// <param name="isShowLoadingImage">�Ƿ���ʾͼ��</param>
        public void ShowOpaqueLayer(Control control)
        {
            //return;
            try
            {
                if (!control.Controls.Contains(this.m_OpaqueLayer))
                {
                    this.m_OpaqueLayer.Dock = DockStyle.Fill;
                    control.Controls.Add(this.m_OpaqueLayer);
                    this.m_OpaqueLayer.BringToFront();
                }
                this.m_OpaqueLayer.Enabled = true;
                this.m_OpaqueLayer.Visible = true;
            }
            catch { }
        }

        /// <summary>
        /// �������ֲ�
        /// </summary>
        public void HideOpaqueLayer()
        {
            try
            {
                if (this.m_OpaqueLayer != null)
                {

                    if (this.m_OpaqueLayer.InvokeRequired)
                    {
                        this.m_OpaqueLayer.Invoke(new Action(delegate
                        {
                            this.m_OpaqueLayer.Visible = false;
                            this.m_OpaqueLayer.Enabled = false;

                        }));
                    }
                    else
                    {
                        this.m_OpaqueLayer.Visible = false;
                        this.m_OpaqueLayer.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("�������ֲ����" + ex.ToString());
            }
        }

    }
}
