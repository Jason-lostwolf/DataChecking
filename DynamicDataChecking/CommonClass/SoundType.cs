using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataChecking
{
    public enum SoundType
    {
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 无数据
        /// </summary>
        NoData,
        /// <summary>
        /// 重复数据
        /// </summary>
        SameData,
        /// <summary>
        /// 登记成功
        /// </summary>
        Succeed,

        FinishOneInnerBox,
        ScanFirstInnerBox,
        BarcodeNotSuccess,
        BarcodeSuccessScanColorBox,
        OneOutsiteBoxFinish,
        OneBoxNotFinish
    }
}
