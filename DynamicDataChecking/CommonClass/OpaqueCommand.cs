
using DynamicDataChecking.Common;
using System;
using System.Windows.Forms;

namespace DynamicDataChecking.CommonClass
{
    public class OpaqueCommand
    {
        private MyOpaqueLayer.MyOpaqueLayer m_OpaqueLayer = null;//°ëÍ¸Ã÷ÃÉ°å²ã

        public OpaqueCommand(int alpha, bool isShowLoadingImage)
        {
            if (m_OpaqueLayer == null)
            {
                m_OpaqueLayer = new MyOpaqueLayer.MyOpaqueLayer(alpha, isShowLoadingImage);
            }
        }

        /// <summary>
        /// ÏÔÊ¾ÕÚÕÖ²ã
        /// </summary>
        /// <param name="control">¿Ø¼þ</param>
        /// <param name="alpha">Í¸Ã÷¶È</param>
        /// <param name="isShowLoadingImage">ÊÇ·ñÏÔÊ¾Í¼±ê</param>
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
        /// Òþ²ØÕÚÕÖ²ã
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
                LogHelper.WriteLog("Òþ²ØÕÚÕÖ²ã´íÎó" + ex.ToString());
            }
        }

    }
}
