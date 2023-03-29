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
    public partial class Hra_NewEmpDlg :  ERPBaseForm
    {
        public Hra_NewEmpDlg()
        {
            InitializeComponent();
        }

        // Modal Dialogue 로드 시 세팅
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            cboCONT_DIV.SelectedIndex = 1;
            //fnSetCtlData();
        }

        //부모폼에서 넘어온 Parameter Setting
        public void fnSetCtlData()
        {
            // 파라미터값 설정
            if (ParentParameter is Hashtable)
            {
                Hashtable ht = (Hashtable)ParentParameter;
                //SetData(panMain, ht);               

            }


        }


        //	SHRA_BI_NEWEMPNODLG_Q Search Pannel UI Setting
        private bool fnQRY_SHRA_BI_NEWEMPNODLG_Q(string strWorkType)
		{
            if (ymdHIR_DT.yyyymmdd.Length != 8) return false;
 
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_NEWEMPNODLG_Q cProc = new SHRA_BI_NEWEMPNODLG_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType, 
								ymdHIR_DT.yyyymmdd, 
								txtCORP_CD.Text,
                                cboCONT_DIV.EditValue.ToString()
								);	
											
				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), panMain);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}



        //	SHRA_BI_NEWEMPNODLG_Q DB ParamSetting
        public class SHRA_BI_NEWEMPNODLG_Q : BaseProcClass
        {
            public SHRA_BI_NEWEMPNODLG_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_NEWEMPNODLG_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_HIR_DT", "varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CONT_DIV", "varchar", 6, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_HIR_DT, System.String @p_CORP_CD, System.String @p_CONT_DIV)
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
					@p_HIR_DT,
					@p_CORP_CD,
                    @p_CONT_DIV
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }
        
        //입사일자에 따라 사번 자동 체번
        private void ymdHIR_DT_EditValueChanged(object sender, EventArgs e)
        {
            fnQRY_SHRA_BI_NEWEMPNODLG_Q("Q");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateControls(panMain) == false) return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

      

        private void txtCORP_NM_EditValueChanged(object sender, EventArgs e)
        {
            if (txtCORP_CD.Text.Length == 2 && ymdHIR_DT.yyyymmdd.Length == 8)
            {
                fnQRY_SHRA_BI_NEWEMPNODLG_Q("Q");
            }
            else
            {
                txtEMP_NO.Text = "";
            }
        }

        private void txtRSDN_NO_Leave(object sender, EventArgs e)
        {
            if (txtRSDN_NO.Text.Replace("-", "").Length == 13)
            {
                txtRSDN_NO.Text = ReturnOnlyNumeric(txtRSDN_NO.Text).Substring(0, 6) + "-" + ReturnOnlyNumeric(txtRSDN_NO.Text).Substring(6);
            }
        }

        //숫자만 반환하는 함수
        private String ReturnOnlyNumeric(String pData)
        {
            String strData = "";
            if (pData == null) pData = "";

            for (int i = 0; i < pData.Length; i++)
            {
                if (char.IsNumber(pData, i) == true)
                    strData = strData + pData.Substring(i, 1);
            }

            return strData;
        }

    }
}
