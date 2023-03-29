using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;

using JPlatform.Client.Library.interFace;
using JPlatform.Client;
using JPlatform.Client.Controls;
using JPlatform.Client.JBaseForm;
using JPlatform.Client.ERPBaseForm;

namespace vPlus.erp.HR
{
    /// <summary>
    /// [연차관리 생성 팝업]
    /// 2013.07.19.서진호
    /// </summary>
    public partial class Hrc_VacationCreateDlg : ERPBaseForm
    {
        #region [전역 변수 선언]

        #endregion  //end [전역 변수 선언]

        #region [생성자]
        /// <summary>
        /// 생성자 
        /// </summary>
        public Hrc_VacationCreateDlg()
        {
            InitializeComponent();
        }
        #endregion  //end [생성자]

        #region [Load]
        /// <summary>
        /// Load 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (ParentParameter is Hashtable)
            {
                Hashtable htParameter = (Hashtable)ParentParameter;
                ctxtCORP_CD.EditValue = htParameter["CORP_CD"].ToString();
                ctxtCORP_NM.EditValue = htParameter["CORP_NM"].ToString();
                cymdBASE_YY.EditValue = htParameter["BASE_YY"].ToString();
                cymdBASE_YMD.EditValue = htParameter["BASE_YMD"].ToString();
                ctxtEMP_NO.EditValue = htParameter["EMP_NO"].ToString();
                ctxtEMP_NM.EditValue = htParameter["EMP_NM"].ToString();
            }
        }
        #endregion

        #region fnSetCtlData - 타 화면에서 호출되어 로드될 경우 Parameter 세팅
        /// <summary>
        /// 타 화면에서 호출되어 로드될 경우 Parameter 세팅
        /// </summary>
        public void fnSetCtlData()
        {
            // 파라미터값 설정
            if (ParentParameter is Hashtable)
            {
                Hashtable ht = (Hashtable)ParentParameter;

                //txtCD_ACNTUNIT.Text = ht["CD_ACNTUNIT"].ToString();
            }

            // 보통의 경우 타 화면에서 호출되어 로드 시 자동 조회
            //if (txtCD_ACNTUNIT.Text.Trim() != "" && numNO_AREA.Text.Trim() != "" )
            //{
            //    QueryClick();
            //}
        }
        #endregion

        #region [Other Button Control]

        #region btnOK_Click - 확인 버튼
        /// <summary>
        /// 확인 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            //연차관리 생성
            if (fnSET_SHRC_VACATIONMGT_CREATE("N"))
            {
                if (ctxtEMP_NO.Text != "")
                    this.FormResult = ctxtEMP_NO.Text.ToString();
                else
                    this.FormResult = "";

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
        #endregion

        #region btnCancel_Click - 취소 버튼
        /// <summary>
        /// 취소 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #endregion  //end [Other Button Control]


        #region [Start DB Related Code By UIBuilder]

        /// <summary>
        /// 연차 생성
        /// </summary>
        /// <param name="strWorkType"></param>
        /// <returns></returns>
        private bool fnSET_SHRC_VACATIONMGT_CREATE(string strWorkType)
        {
            try
            {
                //유효성검사 
                if (!ValidateControls(panMain)) return false;

                SHRC_VACATIONMGT_CREATE cProc = new SHRC_VACATIONMGT_CREATE();
                DataTable dtData = null;

                dtData = cProc.SetParamData(dtData
                                            , strWorkType
                                            , ctxtCORP_CD.Text              //법인코드
                                            , cymdBASE_YY.yyyy              //적용년도
                                            , cymdBASE_YMD.yyyymmdd         //적용기준일자
                                            , ctxtEMP_NO.Text               //사번
                                            , numBASEVACA_QN.Value          //기본연차
                                            , numSVCVACA_QN.Value          //근속연차
                                            , numXCS_USE_ANVC.Value         //전년도 초과사용 연차      
                                            , SessionInfo.UserID);


                bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), panMain);
                cProc = null;
                return bResult;


            }
            catch (Exception exc)
            {
                SetErrorMessage(exc);
                return false;
            }
        }

        #endregion  //end [Start DB Related Code By FingerAddIn]


        #region [Procedure Information Class By UIBuilder]

        #region Class 연차 생성
        /// <summary>
        /// Class 연차 생성 
        /// </summary>
        public class SHRC_VACATIONMGT_CREATE : BaseProcClass
        {
            public SHRC_VACATIONMGT_CREATE()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRC_VACATIONMGT_CREATE";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));       //작업타입
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));          //법인코드
                _ParamInfo.Add(new ParamInfo("@p_BASE_YY", "Varchar", 4, "Input", typeof(System.String)));          //적용년도
                _ParamInfo.Add(new ParamInfo("@p_BASE_YMD", "Varchar", 8, "Input", typeof(System.String)));         //적용기준일자
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));          //사번
                _ParamInfo.Add(new ParamInfo("@p_BASEVACA_QN", "Decimal", 9, "Input", typeof(System.Decimal)));     //기본연차
                _ParamInfo.Add(new ParamInfo("@p_SVCVACA_QN", "Decimal", 9, "Input", typeof(System.Decimal)));      //근속연차
                _ParamInfo.Add(new ParamInfo("@p_USEDVACA_QN", "Decimal", 9, "Input", typeof(System.Decimal)));     //전년도 초과사용 연차
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));         //유저ID
            }

            public DataTable SetParamData(DataTable dataTable
                                          , System.String @p_work_type
                                          , System.String @p_CORP_CD
                                          , System.String @p_BASE_YY
                                          , System.String @p_BASE_YMD
                                          , System.String @p_EMP_NO
                                          , System.Decimal @p_BASEVACA_QN
                                          , System.Decimal @p_SVCVACA_QN
                                          , System.Decimal @p_USEDVACA_QN
                                          , System.String @p_USER_ID)
            {
                if (dataTable == null)
                {
                    dataTable = new DataTable(_ProcName);
                    foreach (ParamInfo pi in _ParamInfo)
                    {
                        dataTable.Columns.Add(pi.ParamName, pi.TypeClass);
                    }
                }
                // Modify Code : Procedure Parameter
                object[] objData = new object[] {
					@p_work_type,
					@p_CORP_CD,
					@p_BASE_YY,
					@p_BASE_YMD,
					@p_EMP_NO,
                    @p_BASEVACA_QN,
                    @p_SVCVACA_QN,
                    @p_USEDVACA_QN,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }
        #endregion  //end Class 연차 생성

        // 사번 입력하면 기본연차 근속연차 입력가능
        private void ctxtEMP_NO_EditValueChanged(object sender, EventArgs e)
        {
            if (ctxtEMP_NO.Text != "")
            {
                numBASEVACA_QN.Properties.ReadOnly = false;
                numSVCVACA_QN.Properties.ReadOnly = false;
                numXCS_USE_ANVC.Properties.ReadOnly = false;

                numBASEVACA_QN.BackColor = System.Drawing.Color.White;
                numSVCVACA_QN.BackColor = System.Drawing.Color.White;
                numXCS_USE_ANVC.BackColor = System.Drawing.Color.White;
            }
            else
            {
                numBASEVACA_QN.Value = 0;
                numSVCVACA_QN.Value = 0;
                numXCS_USE_ANVC.Value = 0;

                numBASEVACA_QN.Properties.ReadOnly = true;
                numSVCVACA_QN.Properties.ReadOnly = true;
                numXCS_USE_ANVC.Properties.ReadOnly = true;

                numBASEVACA_QN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
                numSVCVACA_QN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));
                numXCS_USE_ANVC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(247)))));  
            }

        }

        #endregion //end [Procedure Information Class By FingerAddIn]

    }
}
