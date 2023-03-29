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
using JPlatform.Client.ERPBaseFormInherit;


//PictureBox Image Load할때 Byte[] 배열로 세팅하기 위해서 필요
using System.Drawing.Imaging;
using DevExpress.XtraEditors.Controls;

namespace vPlus.erp.HR
{
	public partial class Hra_BaseInfo : ERPBaseFormInherit
	{
		GridControlEx SelGird;
		GridViewEx SelView;
		String strSCLS_CD1 = "";
		String strSCLS_CD2 = "";
        private bool bZpFocusedColumn = false;

		public Hra_BaseInfo()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			fnSetToolbarButton("INIT");
			fnFormInitSetting();    // 화면로드 시 기본값 설정

			fnSetCtlData();       // 타 화면에서 호출되어 로드될 경우 Parameter 세팅

			//WorkFlow 호출
			fnRibbonInit(ribWorkflow);

			EnabledControls(panSearch, true);	// 프리폼(panInfo1, panInfo2…)이 있는 경우는 필수
			//txtCURY_CD1.Focus();		        // 조회판넬 맨 첫번 째 Control에 포커스
			menuStrip_Family.Visible = true;
            
            //조회조건에 따른 법인, 사번 셋팅  20150126 SDLee
            //if ((FormAccessInfo.SearchLevel == "9"))
            //{
            //    //법인명 셋팅
            //    ctxtCORP_CD.Text = SessionInfo.CORP_CD;
            //    ctxtCORP_NM.Text = SessionInfo.CORP_NM;
            //    //사번 셋팅
            //    ctxtEMP_NO.Text = SessionInfo.EMP_NO ;
            //    ctxtEMP_NM.Text = SessionInfo.UserName;

            //    ctxtCORP_CD.Enabled = false;
            //    ctxtCORP_NM.Enabled = false;
            //    //사번 셋팅
            //    ctxtEMP_NO.Enabled = false;
            //    ctxtEMP_NM.Enabled = false;

            //    ctxtEMP_NO.Properties.ReadOnly = true;
            //    ctxtEMP_NO.Properties.Popup.BizComponentID = "";
            //    ctxtCORP_CD.Properties.ReadOnly = true;
            //    ctxtCORP_CD.Properties.Popup.BizComponentID = "";

            //    EnabledForms(false);
            //}
            //else
            //{
            //    //법인명 셋팅
            //    //ctxtCORP_CD.Text = "";
            //    //ctxtCORP_NM.Text = "";
            //    //사번 셋팅
            //    ctxtEMP_NO.Text = "";
            //    ctxtEMP_NM.Text = "";
            //}
		}

		#region [Button Control]

		// 상단 기본버튼 컨트롤
		protected void fnSetToolbarButton(string pType)
		{
			bool bQuery = true;
			bool bNew = false;
			bool bAddRow = false;   // 미사용 버튼
			bool bDelRow = false;   // 미사용 버튼
			bool bSave = false;
			bool bDelete = false;
			bool bPreview = false;
			bool bPrint = false;

            if (FormAccessInfo.AllowSave)   // 버튼 기본 컨트롤 함수 상 추가 >> [저장]권한이 있을 경우만 작동하도록
            {
                switch (pType)
                {
                    case "INIT":
                        bQuery = true;                        
                        break;
                    case "QUERYX":
                        bQuery = true;
                        //bNew = true;                        
                        break;
                    case "QUERY":
                        bQuery = true;
                        //bNew = true;
                        bDelete = true;
                        break;
                    case "INSERT":
                        bQuery = true;
                        bNew = true;
                        bDelete = true;
                        bSave = true;
                        break;
                    case "UPDATE":
                        break;
                    case "DELETE":
                        break;
                    case "SAVE":
                        break;
                    default:
                        break;
                }

                bNew = false;
            }

            fnSetSearchLevel();

            QueryButton = bQuery;
            NewButton = bNew;
            AddButton = bAddRow;            // 미사용 버튼
            DeleteRowButton = bDelRow;      // 미사용 버튼
            SaveButton = bSave;
            DeleteButton = bDelete;
            PreviewButton = bPreview;
            PrintButton = bPrint;
		}

		#endregion [Button Control Event]

		#region [Control Setting]

        // 조회수준별 권한 설정
        public void fnSetSearchLevel()
        {
            if  (FormAccessInfo.SearchLevel == "9")
            {
                ctxtEMP_NO.Properties.ReadOnly = true;
                ctxtEMP_NO.Properties.Popup.BizComponentID = "";
                ctxtCORP_CD.Properties.ReadOnly = true;
                ctxtCORP_CD.Properties.Popup.BizComponentID = "";
                ctxtDEPT_CD.Properties.ReadOnly = true;
                ctxtDEPT_CD.Properties.Popup.BizComponentID = "";

                //법인명 셋팅
                ctxtCORP_CD.Text = SessionInfo.CORP_CD;
                ctxtCORP_NM.Text = SessionInfo.CORP_NM;
                //사번 셋팅
                ctxtEMP_NO.Text = SessionInfo.EMP_NO;
                ctxtEMP_NM.Text = SessionInfo.UserName;
                //부서 셋팅
                //ctxtDEPT_CD.Text = SessionInfo.DEPT_CD;
                //ctxtDEPT_NM.Text = SessionInfo.DEPT_NM;

                ctxtCORP_CD.Enabled = false;
                ctxtCORP_NM.Enabled = false;
                
                ctxtEMP_NO.Enabled = false;
                ctxtEMP_NM.Enabled = false;

                ctxtDEPT_CD.Enabled = false;
                ctxtDEPT_NM.Enabled = false;

                EnabledForms(false);
            }
            else if (FormAccessInfo.SearchLevel == "7")
            {                
                ctxtCORP_CD.Properties.ReadOnly = true;
                ctxtCORP_CD.Properties.Popup.BizComponentID = "";
                ctxtDEPT_CD.Properties.ReadOnly = true;
                ctxtDEPT_CD.Properties.Popup.BizComponentID = "";

                //법인명 셋팅
                ctxtCORP_CD.Text = SessionInfo.CORP_CD;
                ctxtCORP_NM.Text = SessionInfo.CORP_NM;
               
                //부서 셋팅
                ctxtDEPT_CD.Text = SessionInfo.DEPT_CD;
                ctxtDEPT_NM.Text = SessionInfo.DEPT_NM;

                ctxtCORP_CD.Enabled = false;
                ctxtCORP_NM.Enabled = false;

                ctxtDEPT_CD.Enabled = false;
                ctxtDEPT_NM.Enabled = false;
            }
            else
            {
                ctxtEMP_NO.Properties.ReadOnly = false;
                ctxtEMP_NO.Properties.Popup.BizComponentID = "P_HR_BASEINFO";
                ctxtCORP_CD.Properties.ReadOnly = false;
                ctxtCORP_CD.Properties.Popup.BizComponentID = "P_CC_CORPN";
                ctxtDEPT_CD.Properties.ReadOnly = false;
                ctxtDEPT_CD.Properties.Popup.BizComponentID = "P_HR_DEPT";
            }
        }

		// 화면로드 시 기본값 설정
		public void fnFormInitSetting()
		{
            if (FormAccessInfo.SearchLevel != "9" || FormAccessInfo.SearchLevel != "7")
            {
                WorkCorpCode("SET");    //법인코드 세팅
            }

            chkSPERD_DIV.EditValue = "N";

			SetLookUp(grdAddress, "TERR_CD", "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0AB'");  //지역구분
			SetLookUp(grdAddress, "ADDR_DIV", "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0AA'");  //주소구분    
			SetLookUp(grdFamilly, colFM_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0FA'");  //가족구분
			SetLookUp(grdFamilly, colEDUCRR_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0SB'");  //학력구분
			SetLookUp(grdFamilly, colJOB_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0JA'");  //직업
			SetLookUp(grdSchool, colEDUCRR_CD1.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0SB'");  //학력구분                
			SetLookUp(grdSchool, colLPLC_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0AB'");  //소재지
			SetLookUp(grdSchool, colMJR_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0SE'");  //전공
			SetLookUp(grdSchool, colSUB_MJR_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0SE'");  //부전공
			SetLookUp(grdSchool, colACDGR_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0SC'");  //학위
			SetLookUp(grdPrize, colRNPNSH_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0PA'");  //상벌구분
			SetLookUp(grdAssociation, colCLUB_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0CA'");  //동호회
			SetLookUp(grdLanguage, colLANG_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0TA'");  //LANGUAGE
			SetLookUp(grdEducation, colEDU_CD.FieldName, "L_HR_CODEMASTER", "A.GROUP_CODE = 'HRZ0EE'");  //교육

		}

		// 타 화면에서 호출되어 로드될 경우 Parameter 세팅
		public void fnSetCtlData()
		{
			// 파라미터값 설정
			if (ParentParameter is Hashtable)
			{
				Hashtable ht = (Hashtable)ParentParameter;

				ctxtCORP_CD.Text = ht["CORP_CD"].ToString();
				ctxtCORP_NM.Text = ht["CORP_NM"].ToString();
				ctxtEMP_NO.Text = ht["EMP_NO"].ToString();
				ctxtEMP_NM.Text = ht["EMP_NM"].ToString();
			}

            ZipCodeConfig();

			// 보통의 경우 타 화면에서 호출되어 로드 시 자동 조회
			if (ctxtCORP_CD.Text.Trim() != "" && ctxtEMP_NO.Text.Trim() != "")
			{
				QueryClick();
			}

			ParentParameter = null;
		}

		// panSearch 상 Control(조회조건) 변경 시 Panel 및 Grid 초기화        // panSearch 상 모든 Control의 EditValueChanged 이벤트에 함수연결
		private void fnCtlChangeData(object sender, EventArgs e)
		{
			this.InitControls_All(panList);
			this.InitControls_All(panInfoBase);  //콘트롤내의 모든 자료 지우기
		}

		// panInfo1 / panTabPage1 상 Control(데이터) 변경 시 panSearch 상 Control(조회조건) Disabled        // panInfo1 / panTabPage1 상 모든 Control의 TextChanged 이벤트에 함수연결
		private void fnEditChangeData(object sender, EventArgs e)
		{
			if ((panInfo1.DataStatus != "Q") || (panDetail.DataStatus != "Q"))
			{
				EnabledControls(panSearch, false);

			}
		}

		// Grid 자료변경 시 panSearch 상 Control(조회조건) Disabled // 그리드뷰의 CellValueChanged 이벤트에 함수연결
		private void fnGridCellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
		{
			if (e.Column.OptionsColumn.AllowEdit == true) EnabledControls(panSearch, false);            
		}

		#endregion [Control Setting]

		#region [Grid Setting]

		// Grid Master Row 변경 시 Panel 및 Grid Detail 조회
		private void gvwMaster_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
		{
			if (e.FocusedRowHandle < 0)
				return;

			// gvwMaster_FocusedRowChanged에 관한 모든 처리 내용을 이 함수 안에 구현
			FocusedRowChanged();
		}

		private void FocusedRowChanged()
		{

			InitControls(grdBaseInfo);
			InitControls_All(panInfoBase);

			fnQRY_SHRA_BASEINFO_Q("Q");
			SetPanelFromGrid(grdBaseInfo, panInfo1);

			txtHGL_PNM.Properties.ReadOnly = true;			

			TabQuery();
			//fnQRY_SHRA_BI_ADDRESS_Q("Q");
			//fnQRY_SHRA_BI_ASSOCIATION_Q("Q");
			//fnQRY_SHRA_BI_CAREER_Q("Q");
			//fnQRY_SHRA_BI_CONCURRENT_Q("Q");
			//fnQRY_SHRA_BI_EDU_Q("Q");
			//fnQRY_SHRA_BI_FAMILY_Q("Q");
			//fnQRY_SHRA_BI_HEALTH_Q("Q");
			//fnQRY_SHRA_BI_LANGUAGE_Q("Q");
			//fnQRY_SHRA_BI_LICENSE_Q("Q");
			//fnQRY_SHRA_BI_MILITARYCAREER_Q("Q");
			//fnQRY_SHRA_BI_ORDER_Q("Q");
			//fnQRY_SHRA_BI_PRIZE_Q("Q");
			//fnQRY_SHRA_BI_SCHCAREER_Q("Q");

			gvwMaster.Focus();

            if (tabBaseInfo.SelectedTabPage.Name == "tabPg_Order")
            {
                int Focus = gvwOrder.FocusedRowHandle;

                gvwOrder.FocusedRowHandle = gvwOrder.RowCount - 1;
                gvwOrder.FocusedRowHandle = Focus;
            }
		}

		// Grid Detail 상 기 저장된 KEY 값 수정 불가 / 신규 Row만 입력 가능
		private void SelView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
		{
			if (e.FocusedRowHandle < 0) return;

			// 오류상태인 경우 Return
			if (SelView == null) return;
			if (SelView.GetValue(SelView.Columns[0].FieldName) == null) return;	// 첫번 째 KEY 값 지정           


			switch (SelView.Name.ToString())
			{
				case "gvwAddress": //주소
					if (SelView.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
					{
						SelView.Columns["ADDR_DIV"].OptionsColumn.AllowEdit = true;        //주소구분
						SelView.Columns["ADDR_DIV"].OptionsColumn.ReadOnly = false;
					}
					else
					{
						SelView.Columns["ADDR_DIV"].OptionsColumn.AllowEdit = false;        //주소구분
						SelView.Columns["ADDR_DIV"].OptionsColumn.ReadOnly = true;
					}
					break;
				case "gvwFamilly":   //가족
					if (SelView.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
					{
						SelView.Columns["FM_CD"].OptionsColumn.AllowEdit = true;           // 가족관계
						SelView.Columns["FM_CD"].OptionsColumn.ReadOnly = false;

						SelView.Columns["RSDN_NO"].OptionsColumn.AllowEdit = true;         // 주민번호
						SelView.Columns["RSDN_NO"].OptionsColumn.ReadOnly = false;
					}
					else
					{
						SelView.Columns["FM_CD"].OptionsColumn.AllowEdit = false;           // 가족관계
						SelView.Columns["FM_CD"].OptionsColumn.ReadOnly = true;

						SelView.Columns["RSDN_NO"].OptionsColumn.AllowEdit = false;         // 주민번호
						SelView.Columns["RSDN_NO"].OptionsColumn.ReadOnly = true;
					}
					break;
				case "gvwSchool":   //학력
					if (SelView.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
					{
						SelView.Columns["EDUCRR_CD"].OptionsColumn.AllowEdit = true;       // 학교구분
						SelView.Columns["EDUCRR_CD"].OptionsColumn.ReadOnly = false;

						SelView.Columns["WHSHOL_STR_DT"].OptionsColumn.AllowEdit = true;   // 재학기간
						SelView.Columns["WHSHOL_STR_DT"].OptionsColumn.ReadOnly = false;
					}
					else
					{
						SelView.Columns["EDUCRR_CD"].OptionsColumn.AllowEdit = false;       // 학교구분
						SelView.Columns["EDUCRR_CD"].OptionsColumn.ReadOnly = true;

						SelView.Columns["WHSHOL_STR_DT"].OptionsColumn.AllowEdit = false;   // 재학기간
						SelView.Columns["WHSHOL_STR_DT"].OptionsColumn.ReadOnly = true;
					}

                    //무학~고재 까지 text 그 이상은 popup
                    if (parseInt32(gvwSchool.GetValue("EDUCRR_CD").ToString()) < 40)
                    {
                        this.colSCHL_NM.ColumnEdit = null;
                        this.colSCHL_NM.Popup.PopupEvent = "";
                    }
                    else
                    {
                        this.colSCHL_NM.ColumnEdit = this.repositoryItemButtonEditEx4;
                        this.colSCHL_NM.Popup.PopupEvent = "DoubleClick,ValueCheck";
                    }
					break;
				case "gvwHealth":   //건강검진
					if (SelView.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
					{
						SelView.Columns["CHKUP_DT"].OptionsColumn.AllowEdit = true;       //건강검진일
						SelView.Columns["CHKUP_DT"].OptionsColumn.ReadOnly = false;
					}
					else
					{
						SelView.Columns["CHKUP_DT"].OptionsColumn.AllowEdit = false;       //건강검진일
						SelView.Columns["CHKUP_DT"].OptionsColumn.ReadOnly = true;
					}
					break;
				case "gvwCareer":   //사외경력
					//없음
					break;
				case "gvwLicense":  //면허
					if (SelView.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
					{
						SelView.Columns["LCN_CD"].OptionsColumn.AllowEdit = true;       //면허구분
						SelView.Columns["LCN_CD"].OptionsColumn.ReadOnly = false;
					}
					else
					{
						SelView.Columns["LCN_CD"].OptionsColumn.AllowEdit = false;       //면허구분
						SelView.Columns["LCN_CD"].OptionsColumn.ReadOnly = true;
					}
					break;
				case "gvwOrder":    //발령
					//없음
					break;
				case "gvwPrize":    //상벌
					if (SelView.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
					{
						SelView.Columns["RNPNSH_DT"].OptionsColumn.AllowEdit = true;       // 상벌일자
						SelView.Columns["RNPNSH_DT"].OptionsColumn.ReadOnly = false;

						SelView.Columns["RNPNSH_CD"].OptionsColumn.AllowEdit = true;   // 상벌코드
						SelView.Columns["RNPNSH_CD"].OptionsColumn.ReadOnly = false;
					}
					else
					{
						SelView.Columns["RNPNSH_DT"].OptionsColumn.AllowEdit = false;       // 상벌일자
						SelView.Columns["RNPNSH_DT"].OptionsColumn.ReadOnly = true;

						SelView.Columns["RNPNSH_CD"].OptionsColumn.AllowEdit = false;   // 상벌코드
						SelView.Columns["RNPNSH_CD"].OptionsColumn.ReadOnly = true;
					}
					break;
				case "gvwAssociation":  //동호회
					if (SelView.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
					{
						SelView.Columns["CLUB_CD"].OptionsColumn.AllowEdit = true;       //동호회코드
						SelView.Columns["CLUB_CD"].OptionsColumn.ReadOnly = false;
					}
					else
					{
						SelView.Columns["CLUB_CD"].OptionsColumn.AllowEdit = false;       //동호회코드
						SelView.Columns["CLUB_CD"].OptionsColumn.ReadOnly = true;
					}
					break;
				case "gvwLanguage":     //외국어   
					//없음
					break;
				case "gvwEducation":    //교육
					if (SelView.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
					{
						SelView.Columns["YY"].OptionsColumn.AllowEdit = true;               // 교육연도
						SelView.Columns["YY"].OptionsColumn.ReadOnly = false;

						SelView.Columns["EDU_CD"].OptionsColumn.AllowEdit = true;           // 교육구분코
						SelView.Columns["EDU_CD"].OptionsColumn.ReadOnly = false;
					}
					else
					{
						SelView.Columns["YY"].OptionsColumn.AllowEdit = false;              // 교육연도
						SelView.Columns["YY"].OptionsColumn.ReadOnly = true;

						SelView.Columns["EDU_CD"].OptionsColumn.AllowEdit = false;          // 교육구분코
						SelView.Columns["EDU_CD"].OptionsColumn.ReadOnly = true;
					}
					break;
				default:
					break;
			}
		}

		//레이아웃 뷰 이동
		private void gvwMaster_BeforeLeaveRow_1(object sender, DevExpress.XtraGrid.Views.Base.RowAllowEventArgs e)
		{
			if (SaveButton == true)
			{
				DialogResult dr = SetYesNoMessageBox("수정한 내용이 있습니다.\r\n저장을 하시고 이동하시겠습니까?\r\n(예:저장후 이동, 아니오:이동)");
				if (dr == System.Windows.Forms.DialogResult.Yes)
				{
					//if (TabSave() == false) e.Allow = false;
					e.Allow = TabSave();
				}
			}
		}

		//발령뷰 FocusedRowChanged
		private void gvwOrder_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
		{
			SetPanelFromGrid(grdOrder, panOrder);
			OrderControl_ReadOnly();

            //// 2017.05.26 김두만대리 요청으로 최종자료 이외의 모든자료 수정가능하도록 수정  start
            ////발령자료는 최신자료부터 순차적으로 지운다
            //if (gvwOrder.FocusedRowHandle == 0 && FormAccessInfo.SearchLevel != "9")
            //{
				subDelete_Order.Enabled = true;
				//EnabledControls(panOrder, true);
				OrderControl_Setting(txtGCLS_CD.Text);
            //}
            //else
            //{
            //    subDelete_Order.Enabled = false;

            //    //if (this.SessionInfo.UserID.ToUpper() == "VPLUSUSER")
            //    //    EnabledControls(panOrder, true);
            //    //else
            //    EnabledControls(panOrder, false);
            //}
            //// 2017.05.26 김두만대리 요청으로 최종자료 이외의 모든자료 수정가능하도록 수정 end  
		}

		#endregion [Grid Setting]

		#region [Other Button Control]

		// Grid Detail 상단 행 추가 버튼
		private void subAdd_Click(object sender, EventArgs e)
		{
			if (SelView.Name.ToString() == "gvwOrder")
			{
				InitControls(panOrder);
				EnabledControls(panOrder, true);
				OrderControl_ReadOnly();
				panOrder.DataStatus = "N";
				SaveButton = true;
			}
			else
			{
				// Grid Detail에 행 추가
				GridAddNewRow(SelGird, SelView.RowCount);
				SelView.FocusedRowHandle = SelView.RowCount - 1;

				// Grid Detail의 특정 Column 기본값 설정
				SelView.SetValue("CORP_CD", txtCORP_CD.Text);
				SelView.SetValue("EMP_NO", txtEMP_NO.Text);

				//if (SelView == gvwOrder) SelView.SetValue("PAPP_CORP_CD", txtCORP_CD.Text);

				SelView.ShowEditor();
				EnabledControls(panSearch, false);
			}
		}

		// Grid Detail 상단 행 삭제 버튼
		private void subDelete_Click(object sender, EventArgs e)
		{
			// Grid Detail의 포커스된 행 삭제
			GridDeleteRow(SelGird, false);
			EnabledControls(panSearch, false);
		}

		//발령정보 삭제
		private void subDelete_Order_Click(object sender, EventArgs e)
		{
			if (gvwOrder.FocusedRowHandle != 0) return;
			//DialogResult dr = SetYesNoMessageBox("발령정보를 삭제 하시겠습니까?");
			//if (dr == System.Windows.Forms.DialogResult.Yes)
			//{
			panOrder.DataStatus = "D";
			SaveButton = true;
			//SaveClick();
			//}
		}

		//Military Delete button Click
		private void SubDelete_Military_Click(object sender, EventArgs e)
		{
			InitControls(panMilitary);
			panMilitary.DataStatus = "D";
			SaveButton = true;
		}

		//신규사원등록
		private void btnJumpCreateNewEmp_Click(object sender, EventArgs e)
		{
			// 이동할 화면으로 넘겨줄 값 설정
			//Hashtable ParamHt = new Hashtable();
			tabBaseInfo.SelectedTabPage = tabPg_Detail_Concur;
			InitControls(grdBaseInfo);
			InitControls_All(panInfoBase);

			txtHGL_PNM.Properties.ReadOnly = false;
			txtRSDN_NO.Properties.ReadOnly = false;

			Hra_NewEmpDlg ShowDlg = new Hra_NewEmpDlg();

			ShowDlg._browserMain = this._browserMain;
			//ShowDlg.ParentParameter = ParamHt;

			ShowDlg.txtCORP_CD.Text = this.ctxtCORP_CD.Text.Trim();
			ShowDlg.txtCORP_NM.Text = this.ctxtCORP_NM.Text.Trim();

			if (ShowDlg.ShowDialog() == DialogResult.OK)
			{
				this.txtCORP_CD.Text = ShowDlg.txtCORP_CD.Text.Trim();
				this.txtEMP_NO.Text = ShowDlg.txtEMP_NO.Text.Trim();
				this.txtHGL_PNM.Text = ShowDlg.txtHGL_PNM.Text.Trim();
				this.txtDEPT_CD.Text = ShowDlg.txtDEPT_CD.Text.Trim();
				this.txtDEPT_NM.Text = ShowDlg.txtDEPT_NM.Text.Trim();			
				this.ymdHIR_DT.Text = ShowDlg.ymdHIR_DT.Text.Trim();
				this.txtPAYSTEP_CD.Text = ShowDlg.txtPAYSTEP_CD.Text.Trim();
				this.txtGRADE_CD.Text = ShowDlg.txtGRADE_CD.Text.Trim();
				this.txtGRADE_NM.Text = ShowDlg.txtGRADE_NM.Text.Trim();
				this.txtPOS_NM.Text = ShowDlg.txtPOS_NM.Text.Trim();
				this.cboCONT_DIV.EditValue = ShowDlg.cboCONT_DIV.EditValue.ToString();  //계약구분				
				this.cboSPERD_DIV.EditValue = "01";                                     //재직
				this.cboDTY_CD.EditValue = ShowDlg.txtDTY_CD.Text.Trim();               //직책				
				this.cboJBCLS_CD.EditValue = ShowDlg.txtJBCLS_CD.Text.Trim();           //직종
				this.strSCLS_CD1 = ShowDlg.txtSCLS_CD1.Text.Trim();                     //채용발령
				this.strSCLS_CD2 = ShowDlg.txtSCLS_CD2.Text.Trim();                     //직급부여
                this.txtRSDN_NO.Text = ShowDlg.txtRSDN_NO.Text.Trim();                     //주민번호               
                this.txt_EMP_NO_OLD.Text = ShowDlg.txtEMP_NO_OLD.Text.Trim();                     //이전사번

				picPHOTO.EditValue = ByteImageConverter.ToByteArray(picNo_Img.Image, ImageFormat.Jpeg);

				panInfo1.DataStatus = "N";
				SaveButton = true;
			}
			else
			{
				QueryClick();
			}
		}

		//사진등록
		private void btnImage_Click(object sender, EventArgs e)
		{
			openFileDialog.FileName = "";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = true;
			openFileDialog.Filter = "Image Files | *.jpeg;*.jpg;*.gif;*.png;*.bmp;*.pic;*.tif;*.tiff|All Files |*.*";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				//System.IO.FileInfo imgInfo = new System.IO.FileInfo(openFileDialog.FileName);
				//if (imgInfo.Length / 60 >= 1024) //60Kbyte
				//{
				//    SetMessageBox("사진용량이 60Kbyte를 초과할수 없습니다");
				//    return;
				//}

				//Byte[] 형식으로 Load And ThumbNail 
				Image Tumbimg = Image.FromFile(openFileDialog.FileName).GetThumbnailImage(94, 124, delegate { return false; }, IntPtr.Zero);
                				
                picPHOTO.EditValue = ByteImageConverter.ToByteArray(Tumbimg, ImageFormat.Jpeg);

				if (panInfo1.DataStatus != "N") panInfo1.DataStatus = "U";
				if (SaveButton == false) SaveButton = true;

			}
		}

		private void panOrder_EditValueChanged(object sender, EventArgs e)
		{
			if (panOrder.DataStatus != "U")
				panOrder.DataStatus = "U";
		}

		#endregion [Other Button Control]

		#region [Start Button Event Code By UIBuilder]

		public override void QueryClick()
		{

			int iBeforeHandle = gvwMaster.FocusedRowHandle;

			this.fnCtlChangeData(this, new EventArgs());

			// 조회함수 호출
			fnQRY_SHRA_BI_LAYOUTVIEW_Q("Q");
			if (gvwMaster.RowCount != 0)			// 조회내용이 있을 경우
			{
				fnSetToolbarButton("QUERY");
				// FocusedRowChanged() 이벤트를 사용할 경우에 한하여 작성
				if (iBeforeHandle == 0 && gvwMaster.FocusedRowHandle == 0)
					FocusedRowChanged();
                btnJumpCreateNewEmp.Enabled = true;
			}
			else					// 조회내용이 없을 경우
			{
				fnSetToolbarButton("QUERYX");
			}
			EnabledControls(panSearch, true);		// 조회 후 조회조건 Enabled = True
		}

		public override void SaveClick()
        {
			String SelEmpno = txtEMP_NO.Text.ToString();    //저장전 사번 저장

			if (TabSave() == false) return;
			EnabledControls(panSearch, true);
			FocusedRowChanged();

			if (gvwMaster.GetFocusedRowCellDisplayText("EMP_NO").ToString() == SelEmpno.ToString())
			{
				gvwMaster.SetRowCellValue(gvwMaster.FocusedRowHandle, colPHOTO, picPHOTO.EditValue);
				gvwMaster.SetRowCellValue(gvwMaster.FocusedRowHandle, "GRADE_NM", gvwBaseinfo.GetValue("GRADE_NM"));
				gvwMaster.SetRowCellValue(gvwMaster.FocusedRowHandle, "DEPT_NM", gvwBaseinfo.GetValue("DEPT_NM"));
			}
			else
			{
				QueryClick();
				//gvwMaster.FocusedRowHandle = GetGridRowIndex(grdMaster, "EMP_NO", SelEmpno);
				for (int i = 0; i < gvwMaster.RowCount; i++)
				{
					if (gvwMaster.GetRowCellValue(i, "EMP_NO").ToString() == SelEmpno)
					{
						gvwMaster.FocusedRowHandle = i;
						break;
					}
				}

			}
		}


		public override void DeleteClick()
		{
			// Detail 자료가 있을 경우 먼저 Detail 자료 삭제 후 Master 자료 삭제 (필요 시)
			//if (gvwDetail.RowCount != 0)
			//{
			//    SetMessageBox("발령소분류 코드가 존재합니다.\r\n삭제하실 수 없습니다!");
			//    return;
			//}

			//if (SetYesNoMessageBox("발령대분류 코드를 삭제하시겠습니까?") == DialogResult.Yes)
			//{
			//    // Yes 버튼 클릭 시
			//    if (fnSET_SHRZ_ORDER_S("D") == true)    //발령소분류삭제
			//    {
			//        if (fnSET_SHRZ_CODE_MASTER_S("D") == true)  //발령대분류 삭제
			//            QueryClick();		// 저장 성공 시 재 조회
			//    }
			//}   

			if (SetYesNoMessageBox("사원정보를 삭제하시겠습니까") == DialogResult.Yes)
			{
				panInfo1.DataStatus = "D";

				if (fnSET_SHRA_BASEINFO_S(panInfo1.DataStatus) == false) return;
                
				QueryClick();
			}
		}


		#endregion [End Button Event]

		#region [Start DB Related Code By UIBuilder]

		//Master Grid 조회(Layout View)
		private bool fnQRY_SHRA_BI_LAYOUTVIEW_Q(string strWorkType)
		{
			if (!ValidateControls(panSearch))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_LAYOUTVIEW_Q cProc = new SHRA_BI_LAYOUTVIEW_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								ctxtCORP_CD.Text,
								ctxtEMP_NO.Text,
								ccboCONT_DIV.EditValue.ToString(),
								chkSPERD_DIV.EditValue.ToString(),
                                ctxtDEPT_CD.EditValue.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdMaster);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//Grid Baeinfo Search
		private bool fnQRY_SHRA_BASEINFO_Q(string strWorkType)
		{
			if (!ValidateControls(panSearch))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BASEINFO_Q cProc = new SHRA_BASEINFO_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								gvwMaster.GetFocusedRowCellDisplayText("CORP_CD").ToString(),
								gvwMaster.GetFocusedRowCellDisplayText("EMP_NO").ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdBaseInfo);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//겸직정보 조회
		private bool fnQRY_SHRA_BI_CONCURRENT_Q(string strWorkType)
		{
			if (!ValidateControls(panConcurrent))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_CONCURRENT_Q cProc = new SHRA_BI_CONCURRENT_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdConcurrent);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//주소 조회
		private bool fnQRY_SHRA_BI_ADDRESS_Q(string strWorkType)
		{
			if (!ValidateControls(panAddress))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_ADDRESS_Q cProc = new SHRA_BI_ADDRESS_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdAddress);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//가족사항 조회
		private bool fnQRY_SHRA_BI_FAMILY_Q(string strWorkType)
		{
			if (!ValidateControls(panFamily))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_FAMILY_Q cProc = new SHRA_BI_FAMILY_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdFamilly);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//학력 정보 조회
		private bool fnQRY_SHRA_BI_SCHCAREER_Q(string strWorkType)
		{
			if (!ValidateControls(panSchool))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_SCHCAREER_Q cProc = new SHRA_BI_SCHCAREER_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdSchool);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//건강검진 조회
		private bool fnQRY_SHRA_BI_HEALTH_Q(string strWorkType)
		{
			if (!ValidateControls(panHealth))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_HEALTH_Q cProc = new SHRA_BI_HEALTH_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdHealth);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//경력사항 조회
		private bool fnQRY_SHRA_BI_CAREER_Q(string strWorkType)
		{
			if (!ValidateControls(panCareer))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_CAREER_Q cProc = new SHRA_BI_CAREER_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdCareer);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		// 병역정보 조회
		private bool fnQRY_SHRA_BI_MILITARYCAREER_Q(string strWorkType)
		{
			if (!ValidateControls(panMilitary))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_MILITARYCAREER_Q cProc = new SHRA_BI_MILITARYCAREER_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), panMilitary);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//자격사항 조회
		private bool fnQRY_SHRA_BI_LICENSE_Q(string strWorkType)
		{
			if (!ValidateControls(panLicense))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_LICENSE_Q cProc = new SHRA_BI_LICENSE_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdLicense);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//발령정보 조회
		private bool fnQRY_SHRA_BI_ORDER_Q(string strWorkType)
		{
			if (!ValidateControls(panOrder_Grid))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_ORDER_Q cProc = new SHRA_BI_ORDER_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdOrder);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//상벌정보 조회
		private bool fnQRY_SHRA_BI_PRIZE_Q(string strWorkType)
		{
			if (!ValidateControls(panPrize))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_PRIZE_Q cProc = new SHRA_BI_PRIZE_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdPrize);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//동호회 조회
		private bool fnQRY_SHRA_BI_ASSOCIATION_Q(string strWorkType)
		{
			if (!ValidateControls(panAssociation))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_ASSOCIATION_Q cProc = new SHRA_BI_ASSOCIATION_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdAssociation);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//외국어정보 조회
		private bool fnQRY_SHRA_BI_LANGUAGE_Q(string strWorkType)
		{
			if (!ValidateControls(panLanguage))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_LANGUAGE_Q cProc = new SHRA_BI_LANGUAGE_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdLanguage);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//교육정보 조회
		private bool fnQRY_SHRA_BI_EDU_Q(string strWorkType)
		{
			if (!ValidateControls(panEducation))
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_EDU_Q cProc = new SHRA_BI_EDU_Q();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text.ToString(),
								txtEMP_NO.Text.ToString());

				CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdEducation);
				cProc = null;
				return true;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//BaseInfo 저장
		private bool fnSET_SHRA_BASEINFO_S(string strWorkType)
		{
			if (strWorkType == "Q") return true;

			if (!ValidateControls(panInfo1)) return false;
            if (!ValidateControls(panDetail)) return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BASEINFO_S cProc = new SHRA_BASEINFO_S();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text,
								txtEMP_NO.Text,
								txtHGL_PNM.Text,
								"",
								txtENG_PNM.Text,
								txtDEPT_CD.EditValue.ToString(),
								txtRSDN_NO.Text.Replace("-", ""),
								"",
								ymdHIR_DT.yyyymmdd,
								ymdRTR_DT.yyyymmdd,
								cboSPERD_DIV.EditValue.ToString(),
								cboCONT_DIV.EditValue.ToString(),
                                "",
								txtPAYSTEP_CD.Text,
								txtPRE_PAYSTEP.Text,
								txtGRADE_CD.Text,
								cboJBCLS_CD.EditValue.ToString(),
								cboDTY_CD.EditValue.ToString(),
								cboJB_CD.EditValue.ToString(),
								cboADP_DIV.EditValue.ToString(),
								ymdBRTH.yyyymmdd,
								radYINNYANG_DIV.EditValue.ToString(),
                                radGENDER_TY.EditValue.ToString(),
								cboRLGN_CD.EditValue.ToString(),
								txtHOU_TEL_NO.Text,
								txtCELPH_NO.Text,
								txtCO_TEL_NO.Text,
                                txtUSERID.Text,
								txtEMAIL_ADDR.Text,
								txtHPG_ADDR.Text,
								txtHBY.Text,
								txtSMNT.Text,
								txtDISP_DEPT_CD.EditValue.ToString(),
								ymdDISP_STR_DT.yyyymmdd,
								ymdDISP_END_DT.yyyymmdd,
								ymdMARG_ANNI.yyyymmdd,
								radMARG_YN.EditValue.ToString(),
								chkOBST_YN.EditValue.ToString(),
								chkVETER_YN.EditValue.ToString(),
								radANNS_YN.EditValue.ToString(),
								picPHOTO.EditValue is System.DBNull ? null : (Byte[])picPHOTO.EditValue,
								ymdOBST_DT.yyyymmdd,
								txtOBST_DS.Text,
								strSCLS_CD1,
								strSCLS_CD2,
								Encryption(ReturnOnlyNumeric(txtRSDN_NO.Text).Substring(6)),     //주민번호 뒷자리 암호화
								GetClientPCName() + "/" + GetIPAddress(),   //입력자 pc 정보
								SessionInfo.UserID,
                                txtCORP_OLD_CD.Text,
                                txt_EMP_NO_OLD.Text
                                );

				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), panInfo1);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//주소정보 저장
		private bool fnSET_SHRA_BI_ADDRESS_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_ADDRESS_S cProc = new SHRA_BI_ADDRESS_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdAddress, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;

				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									dr["ADDR_DIV"].ToString(),
									dr["TERR_CD"].ToString(),
									dr["ZPCD"].ToString(),
									dr["ADDR"].ToString(),
									SessionInfo.UserID);

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdAddress);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//가족사항 저장
		private bool fnSET_SHRA_BI_FAMILY_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_FAMILY_S cProc = new SHRA_BI_FAMILY_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdFamilly, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;



				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									dr["RSDN_NO"].ToString().Replace("-", ""),
									dr["FM_CD"].ToString(),
									dr["PNM"].ToString(),
									dr["EDUCRR_CD"].ToString(),
									dr["JOB_CD"].ToString(),
									SessionInfo.UserID);

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdFamilly);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//학력정보 저장
		private bool fnSET_SHRA_BI_SCHCAREER_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_SCHCAREER_S cProc = new SHRA_BI_SCHCAREER_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdSchool, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;

				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									dr["EDUCRR_CD"].ToString(),
									dr["WHSHOL_STR_DT"].ToString().Replace("-", ""),
									dr["WHSHOL_END_DT"].ToString().Replace("-", ""),
									dr["SCHL_CD"].ToString(),
									dr["SCHL_NM"].ToString(),
									dr["LPLC_CD"].ToString(),
									dr["MJR_CD"].ToString(),
									dr["SUB_MJR_CD"].ToString(),
									dr["ACDGR_CD"].ToString(),
									dr["RMKS"].ToString(),
									SessionInfo.UserID);

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdSchool);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//건강검진 저장
		private bool fnSET_SHRA_BI_HEALTH_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_HEALTH_S cProc = new SHRA_BI_HEALTH_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdHealth, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;

				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									dr["CHKUP_DT"].ToString().Replace("-", ""),
									Int32.Parse(dr["HEIGHT"].ToString()),
									Int32.Parse(dr["BWT"].ToString()),
									Decimal.Parse(dr["EYST_LEFT"].ToString()),
									Decimal.Parse(dr["EYST_RIGHT"].ToString()),
									Decimal.Parse(dr["HEARING_LEFT"].ToString()),
									Decimal.Parse(dr["HEARING_RIGHT"].ToString()),
									Int32.Parse(dr["BLPR_HGH"].ToString()),
									Int32.Parse(dr["BLPR_LOW"].ToString()),
									dr["COLBLD_YN"].ToString(),
									dr["BTP"].ToString(),
									dr["HLTH_STATS"].ToString(),
									SessionInfo.UserID,
                                    dr["CHKUP_YN"].ToString());

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdHealth);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//경력사항 저장
		private bool fnSET_SHRA_BI_CAREER_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_CAREER_S cProc = new SHRA_BI_CAREER_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdCareer, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;

				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									Int32.Parse(dr["SEQ"].ToString()),
									dr["EMPLC"].ToString(),
									dr["WRK_STR_DT"].ToString().Replace("-", ""),
									dr["WRK_END_DT"].ToString().Replace("-", ""),
									dr["SIT_NM"].ToString(),
									dr["POS"].ToString(),
									dr["JB"].ToString(),
									dr["TASK"].ToString(),
									0,//Int16.Parse(dr["RCG_WRK_YCNT"].ToString()),
									0,//Decimal.Parse(dr["RCG_WRK_MCNT"].ToString()),
									dr["RMKS"].ToString(),
									SessionInfo.UserID);

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdCareer);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//병역정보 저장
		private bool fnSET_SHRA_BI_MILITARYCAREER_S(string strWorkType)
		{
			if (!ValidateControls(panMilitary))
				return false;

			if (strWorkType == "Q") return true;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_MILITARYCAREER_S cProc = new SHRA_BI_MILITARYCAREER_S();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text,
								txtEMP_NO.Text,
								cboRSC_DIV.EditValue.ToString(),
								cboGUNB_CD.EditValue.ToString(),
								cboBUNGG_CD.EditValue.ToString(),
								cboGRADE_CD.EditValue.ToString(),
								cboGEDE_DIV_CD.EditValue.ToString(),
								ymdIPDE_DT.yyyymmdd,
								ymdGEDE_DT.yyyymmdd,
								SessionInfo.UserID);

				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), panMilitary);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//자격사항 저장
		private bool fnSET_SHRA_BI_LICENSE_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_LICENSE_S cProc = new SHRA_BI_LICENSE_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdLicense, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;

				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									dr["LCN_CD"].ToString(),
									dr["LCN_NO"].ToString(),
									dr["ACQ_DD"].ToString().Replace("-", ""),
									dr["PER_ORG"].ToString(),
									dr["VLD_PERD"].ToString().Replace("-", ""),
									dr["ALLW_PAY_YN"].ToString(),
									SessionInfo.UserID);

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdLicense);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//발령정보 저장
		private bool fnSET_SHRA_BI_ORDER_S(string strWorkType)
		{
			if (ValidateControls(panOrder) == false)
				return false;

			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_ORDER_S cProc = new SHRA_BI_ORDER_S();
				DataTable dtData = null;
				dtData = cProc.SetParamData(dtData,
								strWorkType,
								txtCORP_CD.Text,
								txtEMP_NO.Text,
								ymdPAPP_DT.yyyymmdd,
								Int32.Parse(numPAPP_SO.Value.ToString()),
								txtPAPP_CORP_CD.Text,
								txtGCLS_CD.Text,
								txtSCLS_CD.Text,
								null,
								txtDEPT_CD1.Text,
								txtPAYSTEP_CD1.Text,
								txtGRADE_CD1.Text,
								txtDTY_CD.Text,
								txtJB_CD.Text,
								null,
								null,
								null,
								txtDOC_NO.Text,
								txtRMKS.Text,
								SessionInfo.UserID);

				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), panOrder);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//상벌사항 저장
		private bool fnSET_SHRA_BI_PRIZE_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_PRIZE_S cProc = new SHRA_BI_PRIZE_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdPrize, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;

				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									dr["RNPNSH_DT"].ToString().Replace("-", ""),
									dr["RNPNSH_CD"].ToString(),
									dr["RSN"].ToString(),
									dr["RSLT"].ToString(),
									SessionInfo.UserID);

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdPrize);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//동호회 저장
		private bool fnSET_SHRA_BI_ASSOCIATION_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_ASSOCIATION_S cProc = new SHRA_BI_ASSOCIATION_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdAssociation, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;

				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									dr["CLUB_CD"].ToString(),
									dr["JN_DD"].ToString().Replace("-", ""),
									dr["DUES_EN"].ToString(),
									Int32.Parse(dr["DUES"].ToString()),
									dr["RMKS"].ToString(),
									SessionInfo.UserID);

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdAssociation);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//외국어 저장
		private bool fnSET_SHRA_BI_LANGUAGE_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_LANGUAGE_S cProc = new SHRA_BI_LANGUAGE_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdLanguage, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;

				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									Int32.Parse(dr["SEQ"].ToString()),
									dr["LANG_CD"].ToString(),
                                    dr["TEST_CD"].ToString(),
									dr["LANG_SCR"].ToString(),
									dr["ACQ_DT"].ToString().Replace("-", ""),
									dr["ETM_DT"].ToString().Replace("-", ""),
									dr["RMKS"].ToString(),
									SessionInfo.UserID);

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdLanguage);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		//교육사항 저장
		private bool fnSET_SHRA_BI_EDU_S(string strWorkType)
		{
			try
			{
				// 비즈니스 로직 정보
				SHRA_BI_EDU_S cProc = new SHRA_BI_EDU_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdEducation, true, false);
				if (dtSource == null) return false;
				if (dtSource.Rows.Count == 0) return true;

				foreach (DataRow dr in dtSource.Rows)
				{
					dtData = cProc.SetParamData(dtData,
									dr["RowStatus"].ToString(),
									dr["CORP_CD"].ToString(),
									dr["EMP_NO"].ToString(),
									dr["YY"].ToString(),
									Int32.Parse(dr["SEQ"].ToString()),
									dr["EDU_CD"].ToString(),
									dr["EDU_STR_DT"].ToString().Replace("-", ""),
									dr["EDU_END_DT"].ToString().Replace("-", ""),
									dr["EDU_EXP"].ToString(),
									dr["EDU_TM"].ToString(),
									dr["EDU_ORG"].ToString(),
									dr["GVN_CRED"].ToString(),
									dr["ACQ_CRED"].ToString(),
									dr["EDU_NM"].ToString(),
									dr["OSF_CMPL_YN"].ToString(),
									dr["CHR_CMPL_YN"].ToString(),
									dr["RMKS"].ToString(),
									SessionInfo.UserID);

				}
				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdEducation);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

		#endregion [End DB Related Code]

		#region [Procedure Information Class By UIBuilder]


		//	SHRA_BI_LAYOUTVIEW_Q DB ParamSetting
		public class SHRA_BI_LAYOUTVIEW_Q : BaseProcClass
		{
			public SHRA_BI_LAYOUTVIEW_Q()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_LAYOUTVIEW_Q";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "varchar", 15, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CONT_DIV", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_SPERD_DIV", "varchar", 4, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_DEPT_CD", "varchar", 6, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                            System.String @p_CONT_DIV, System.String @p_SPERD_DIV, System.String @p_DEPT_CD)
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
					@p_EMP_NO,
					@p_CONT_DIV,
					@p_SPERD_DIV,
                    @p_DEPT_CD
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

		public class SHRA_BASEINFO_Q : BaseProcClass
		{
			public SHRA_BASEINFO_Q()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BASEINFO_Q";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}


		//	SHRA_BASEINFO_S DB ParamSetting
		public class SHRA_BASEINFO_S : BaseProcClass
		{
			public SHRA_BASEINFO_S()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BASEINFO_S";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "varchar", 15, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_HGL_PNM", "varchar", 15, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CHNS_PNM", "varchar", 15, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_ENG_PNM", "varchar", 30, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_DEPT_CD", "varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_RSDN_NO", "varchar", 13, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_GRP_HIR_DT", "varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_HIR_DT", "varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_RTR_DT", "varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_SPERD_DIV", "char", 2, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CONT_DIV", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_TP", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_PAYSTEP_CD", "char", 5, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_PRE_PAYSTEP", "char", 5, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_GRADE_CD", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_JBCLS_CD", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_DTY_CD", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_JB_CD", "varchar", 50, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_ADP_DIV", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_BRTH", "varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_YINNYANG_DIV", "char", 1, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_GENDER_TY", "char", 1, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_RLGN_CD", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_HOU_TEL_NO", "varchar", 20, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CELPH_NO", "varchar", 20, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CO_TEL_NO", "varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USERID", "varchar", 20, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMAIL_ADDR", "varchar", 200, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_HPG_ADDR", "varchar", 30, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_HBY", "varchar", 30, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_SMNT", "varchar", 30, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_DISP_DEPT_CD", "varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_DISP_STR_DT", "varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_DISP_END_DT", "varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_MARG_ANNI", "varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_MARG_YN", "char", 1, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_OBST_YN", "char", 1, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_VETER_YN", "char", 1, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_ANNS_YN", "char", 1, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_PHOTO", "image", 16, "Input", typeof(System.Byte[])));
				_ParamInfo.Add(new ParamInfo("@p_OBST_DT", "varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_OBST_DS", "varchar", 100, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_SCLS_CD1", "varchar", 2, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_SCLS_CD2", "varchar", 2, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_PWD_DS", "varchar", 100, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_PC_IF", "nvarchar", 100, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_USER_ID", "varchar", 30, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD_OLD", "varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO_OLD", "varchar", 15, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
											System.String @p_HGL_PNM, System.String @p_CHNS_PNM, System.String @p_ENG_PNM,
											System.String @p_DEPT_CD, System.String @p_RSDN_NO, System.String @p_GRP_HIR_DT,
											System.String @p_HIR_DT, System.String @p_RTR_DT, System.String @p_SPERD_DIV,
											System.String @p_CONT_DIV, System.String @p_EMP_TP, System.String @p_PAYSTEP_CD,
											System.String @p_PRE_PAYSTEP, System.String @p_GRADE_CD, System.String @p_JBCLS_CD,
											System.String @p_DTY_CD, System.String @p_JB_CD, System.String @p_ADP_DIV,
                                            System.String @p_BRTH, System.String @p_YINNYANG_DIV, System.String @p_GENDER_TY, System.String @p_RLGN_CD,
											System.String @p_HOU_TEL_NO, System.String @p_CELPH_NO, System.String @p_CO_TEL_NO, System.String @p_USERID,
											System.String @p_EMAIL_ADDR, System.String @p_HPG_ADDR, System.String @p_HBY,
											System.String @p_SMNT, System.String @p_DISP_DEPT_CD, System.String @p_DISP_STR_DT,
											System.String @p_DISP_END_DT, System.String @p_MARG_ANNI, System.String @p_MARG_YN,
											System.String @p_OBST_YN, System.String @p_VETER_YN, System.String @p_ANNS_YN,
											System.Byte[] @p_PHOTO, System.String @p_OBST_DT, System.String @p_OBST_DS,
											System.String @p_SCLS_CD1, System.String @p_SCLS_CD2, System.String @p_PWD_DS,
                                            System.String @p_PC_IF, System.String @p_USER_ID, System.String @p_CORP_CD_OLD, System.String @p_EMP_NO_OLD)
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
					@p_EMP_NO,
					@p_HGL_PNM,
					@p_CHNS_PNM,
					@p_ENG_PNM,
					@p_DEPT_CD,
					@p_RSDN_NO,
					@p_GRP_HIR_DT,
					@p_HIR_DT,
					@p_RTR_DT,
					@p_SPERD_DIV,
					@p_CONT_DIV,
					@p_EMP_TP,
					@p_PAYSTEP_CD,
					@p_PRE_PAYSTEP,
					@p_GRADE_CD,
					@p_JBCLS_CD,
					@p_DTY_CD,
					@p_JB_CD,
					@p_ADP_DIV,
					@p_BRTH,
					@p_YINNYANG_DIV,
                    @p_GENDER_TY,
					@p_RLGN_CD,
					@p_HOU_TEL_NO,
					@p_CELPH_NO,
					@p_CO_TEL_NO,
                    @p_USERID,
					@p_EMAIL_ADDR,
					@p_HPG_ADDR,
					@p_HBY,
					@p_SMNT,
					@p_DISP_DEPT_CD,
					@p_DISP_STR_DT,
					@p_DISP_END_DT,
					@p_MARG_ANNI,
					@p_MARG_YN,
					@p_OBST_YN,
					@p_VETER_YN,
					@p_ANNS_YN,
					@p_PHOTO,
					@p_OBST_DT,
					@p_OBST_DS,
					@p_SCLS_CD1,
					@p_SCLS_CD2,                    
					@p_PWD_DS,
					@p_PC_IF,
					@p_USER_ID,
                    @p_CORP_CD_OLD,
                    @p_EMP_NO_OLD
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}




		public class SHRA_BI_ADDRESS_Q : BaseProcClass
		{
			public SHRA_BI_ADDRESS_Q()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_ADDRESS_Q";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

		public class SHRA_BI_ADDRESS_S : BaseProcClass
		{
			public SHRA_BI_ADDRESS_S()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_ADDRESS_S";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_ADDR_DIV", "Varchar", 2, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_TERR_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_ZPCD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_ADDR", "Varchar", 200, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
										System.String @p_ADDR_DIV, System.String @p_TERR_CD, System.String @p_ZPCD,
										System.String @p_ADDR, System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_ADDR_DIV,
					@p_TERR_CD,
					@p_ZPCD,
					@p_ADDR,
					@p_USER_ID
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

		public class SHRA_BI_ASSOCIATION_Q : BaseProcClass
		{
			public SHRA_BI_ASSOCIATION_Q()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_ASSOCIATION_Q";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

		public class SHRA_BI_ASSOCIATION_S : BaseProcClass
		{
			public SHRA_BI_ASSOCIATION_S()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_ASSOCIATION_S";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CLUB_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_JN_DD", "Char", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_DUES_EN", "Char", 1, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_DUES", "Int", 4, "Input", typeof(System.Int32)));
				_ParamInfo.Add(new ParamInfo("@p_RMKS", "Varchar", 60, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
										System.String @p_CLUB_CD, System.String @p_JN_DD, System.String @p_DUES_EN,
										System.Int32 @p_DUES, System.String @p_RMKS, System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_CLUB_CD,
					@p_JN_DD,
					@p_DUES_EN,
					@p_DUES,
					@p_RMKS,
					@p_USER_ID
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

		public class SHRA_BI_CAREER_Q : BaseProcClass
		{
			public SHRA_BI_CAREER_Q()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_CAREER_Q";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

		public class SHRA_BI_CAREER_S : BaseProcClass
		{
			public SHRA_BI_CAREER_S()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_CAREER_S";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_SEQ", "Int", 4, "Input", typeof(System.Int32)));
				_ParamInfo.Add(new ParamInfo("@p_EMPLC", "Varchar", 100, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_WRK_STR_DT", "Varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_WRK_END_DT", "Varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_SIT_NM", "Varchar", 300, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_POS", "Varchar", 40, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_JB", "Varchar", 40, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_TASK", "Varchar", 40, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_RCG_WRK_YCNT", "Smallint", 2, "Input", typeof(System.Int16)));
				_ParamInfo.Add(new ParamInfo("@p_RCG_WRK_MCNT", "Decimal", 5, "Input", typeof(System.Decimal)));
				_ParamInfo.Add(new ParamInfo("@p_RMKS", "Varchar", 100, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
										System.Int32 @p_SEQ, System.String @p_EMPLC, System.String @p_WRK_STR_DT,
										System.String @p_WRK_END_DT, System.String @p_SIT_NM, System.String @p_POS,
										System.String @p_JB, System.String @p_TASK, System.Int16 @p_RCG_WRK_YCNT,
										System.Decimal @p_RCG_WRK_MCNT, System.String @p_RMKS, System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_SEQ,
					@p_EMPLC,
					@p_WRK_STR_DT,
					@p_WRK_END_DT,
					@p_SIT_NM,
					@p_POS,
					@p_JB,
					@p_TASK,
					@p_RCG_WRK_YCNT,
					@p_RCG_WRK_MCNT,
					@p_RMKS,
					@p_USER_ID
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

		public class SHRA_BI_CONCURRENT_Q : BaseProcClass
		{
			public SHRA_BI_CONCURRENT_Q()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_CONCURRENT_Q";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

		public class SHRA_BI_EDU_Q : BaseProcClass
		{
			public SHRA_BI_EDU_Q()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_EDU_Q";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
			}

			public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

		public class SHRA_BI_EDU_S : BaseProcClass
		{
			public SHRA_BI_EDU_S()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRA_BI_EDU_S";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
				_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_YY", "Varchar", 4, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_SEQ", "Int", 4, "Input", typeof(System.Int32)));
				_ParamInfo.Add(new ParamInfo("@p_EDU_CD", "Varchar", 6, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EDU_STR_DT", "Varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EDU_END_DT", "Varchar", 8, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EDU_EXP", "Varchar", 40, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EDU_TM", "Char", 10, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_EDU_ORG", "Varchar", 100, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_GVN_CRED", "Varchar", 40, "Input", typeof(System.String)));
				_ParamInfo.Add(new ParamInfo("@p_ACQ_CRED", "Char", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EDU_NM", "VARChar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_OSF_CMPL_YN", "Char", 1, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CHR_CMPL_YN", "Char", 1, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RMKS", "Char", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.String @p_YY, System.Int32 @p_SEQ, System.String @p_EDU_CD,
                                        System.String @p_EDU_STR_DT, System.String @p_EDU_END_DT, System.String @p_EDU_EXP,
                                        System.String @p_EDU_TM, System.String @p_EDU_ORG, System.String @p_GVN_CRED,
                                        System.String @p_ACQ_CRED, System.String @p_EDU_NM, System.String @p_OSF_CMPL_YN,
                                        System.String @p_CHR_CMPL_YN, System.String @p_RMKS, System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_YY,
					@p_SEQ,
					@p_EDU_CD,
					@p_EDU_STR_DT,
					@p_EDU_END_DT,
					@p_EDU_EXP,
					@p_EDU_TM,
					@p_EDU_ORG,
					@p_GVN_CRED,
					@p_ACQ_CRED,
					@p_EDU_NM,
					@p_OSF_CMPL_YN,
					@p_CHR_CMPL_YN,
					@p_RMKS,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_FAMILY_Q : BaseProcClass
        {
            public SHRA_BI_FAMILY_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_FAMILY_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_FAMILY_S : BaseProcClass
        {
            public SHRA_BI_FAMILY_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_FAMILY_S";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RSDN_NO", "Varchar", 13, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_FM_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_PNM", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EDUCRR_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_JOB_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.String @p_RSDN_NO, System.String @p_FM_CD, System.String @p_PNM,
                                        System.String @p_EDUCRR_CD, System.String @p_JOB_CD, System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_RSDN_NO,
					@p_FM_CD,
					@p_PNM,
					@p_EDUCRR_CD,
					@p_JOB_CD,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_HEALTH_Q : BaseProcClass
        {
            public SHRA_BI_HEALTH_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_HEALTH_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_HEALTH_S : BaseProcClass
        {
            public SHRA_BI_HEALTH_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_HEALTH_S";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CHKUP_DT", "Varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_HEIGHT", "Int", 4, "Input", typeof(System.Int32)));
                _ParamInfo.Add(new ParamInfo("@p_BWT", "Int", 4, "Input", typeof(System.Int32)));
                _ParamInfo.Add(new ParamInfo("@p_EYST_LEFT", "Decimal", 9, "Input", typeof(System.Decimal)));
                _ParamInfo.Add(new ParamInfo("@p_EYST_RIGHT", "Decimal", 9, "Input", typeof(System.Decimal)));
                _ParamInfo.Add(new ParamInfo("@p_HEARING_LEFT", "Decimal", 9, "Input", typeof(System.Decimal)));
                _ParamInfo.Add(new ParamInfo("@p_HEARING_RIGHT", "Decimal", 9, "Input", typeof(System.Decimal)));
                _ParamInfo.Add(new ParamInfo("@p_BLPR_HGH", "Int", 4, "Input", typeof(System.Int32)));
                _ParamInfo.Add(new ParamInfo("@p_BLPR_LOW", "Int", 4, "Input", typeof(System.Int32)));
                _ParamInfo.Add(new ParamInfo("@p_COLBLD_YN", "Char", 1, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_BTP", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_HLTH_STATS", "Varchar", 20, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CHCKUP_YN", "Char", 1, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.String @p_CHKUP_DT, System.Int32 @p_HEIGHT, System.Int32 @p_BWT,
                                        System.Decimal @p_EYST_LEFT, System.Decimal @p_EYST_RIGHT, System.Decimal @p_HEARING_LEFT,
                                        System.Decimal @p_HEARING_RIGHT, System.Int32 @p_BLPR_HGH, System.Int32 @p_BLPR_LOW,
                                        System.String @p_COLBLD_YN, System.String @p_BTP, System.String @p_HLTH_STATS,
                                        System.String @p_USER_ID, System.String @p_CHCKUP_YN)
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
					@p_EMP_NO,
					@p_CHKUP_DT,
					@p_HEIGHT,
					@p_BWT,
					@p_EYST_LEFT,
					@p_EYST_RIGHT,
					@p_HEARING_LEFT,
					@p_HEARING_RIGHT,
					@p_BLPR_HGH,
					@p_BLPR_LOW,
					@p_COLBLD_YN,
					@p_BTP,
					@p_HLTH_STATS,
					@p_USER_ID,
                    @p_CHCKUP_YN
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_LANGUAGE_Q : BaseProcClass
        {
            public SHRA_BI_LANGUAGE_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_LANGUAGE_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_LANGUAGE_S : BaseProcClass
        {
            public SHRA_BI_LANGUAGE_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_LANGUAGE_S";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_SEQ", "Int", 4, "Input", typeof(System.Int32)));
                _ParamInfo.Add(new ParamInfo("@p_LANG_CD", "Varchar", 5, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_TEST_CD", "Varchar", 5, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_LANG_SCR", "Varchar", 100, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_ACQ_DT", "Varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_ETM_DT", "Varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RMKS", "Varchar", 1000, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.Int32 @p_SEQ, System.String @p_LANG_CD, System.String @p_TEST_CD, System.String @p_LANG_SCR,
                                        System.String @p_ACQ_DT, System.String @p_ETM_DT, System.String @p_RMKS,
                                        System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_SEQ,
					@p_LANG_CD,
                    @p_TEST_CD,
					@p_LANG_SCR,
					@p_ACQ_DT,
					@p_ETM_DT,
					@p_RMKS,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_LICENSE_Q : BaseProcClass
        {
            public SHRA_BI_LICENSE_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_LICENSE_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));

            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_LICENSE_S : BaseProcClass
        {
            public SHRA_BI_LICENSE_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_LICENSE_S";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_LCN_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_LCN_NO", "Varchar", 40, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_ACQ_DD", "Varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_PER_ORG", "Varchar", 200, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_VLD_PERD", "Varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_ALLW_PAY_YN", "Char", 1, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.String @p_LCN_CD, System.String @p_LCN_NO, System.String @p_ACQ_DD,
                                        System.String @p_PER_ORG, System.String @p_VLD_PERD, System.String @p_ALLW_PAY_YN,
                                        System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_LCN_CD,
					@p_LCN_NO,
					@p_ACQ_DD,
					@p_PER_ORG,
					@p_VLD_PERD,
					@p_ALLW_PAY_YN,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_MILITARYCAREER_Q : BaseProcClass
        {
            public SHRA_BI_MILITARYCAREER_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_MILITARYCAREER_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_MILITARYCAREER_S : BaseProcClass
        {
            public SHRA_BI_MILITARYCAREER_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_MILITARYCAREER_S";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RSC_DIV", "Varchar", 2, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_GUNB_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_BUNGG_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_GRADE_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_GEDE_DIV_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_IPDE_DT", "Varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_GEDE_DT", "Varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.String @p_RSC_DIV, System.String @p_GUNB_CD, System.String @p_BUNGG_CD,
                                        System.String @p_GRADE_CD, System.String @p_GEDE_DIV_CD, System.String @p_IPDE_DT,
                                        System.String @p_GEDE_DT, System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_RSC_DIV,
					@p_GUNB_CD,
					@p_BUNGG_CD,
					@p_GRADE_CD,
					@p_GEDE_DIV_CD,
					@p_IPDE_DT,
					@p_GEDE_DT,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_ORDER_Q : BaseProcClass
        {
            public SHRA_BI_ORDER_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_ORDER_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }


        //	SHRA_BI_ORDER_S DB ParamSetting
        public class SHRA_BI_ORDER_S : BaseProcClass
        {
            public SHRA_BI_ORDER_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_ORDER_S";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_PAPP_DT", "varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_PAPP_SO", "int", 4, "Input", typeof(System.Int32)));
                _ParamInfo.Add(new ParamInfo("@p_PAPP_CORP_CD", "varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_GCLS_CD", "char", 2, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_SCLS_CD", "char", 2, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_PRE_PAPP_DEPT_CD", "varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_DEPT_CD", "varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_PAYSTEP_CD", "varchar", 5, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_GRADE_CD", "varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_DTY_CD", "varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_JB_CD", "varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_DISP_YN", "char", 1, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_DISP_STR_DT", "varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_DISP_END_DT", "varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_DOC_NO", "varchar", 50, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RMKS", "varchar", 225, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "varchar", 30, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                            System.String @p_PAPP_DT, System.Int32 @p_PAPP_SO, System.String @p_PAPP_CORP_CD,
                                            System.String @p_GCLS_CD, System.String @p_SCLS_CD, System.String @p_PRE_PAPP_DEPT_CD,
                                            System.String @p_DEPT_CD, System.String @p_PAYSTEP_CD, System.String @p_GRADE_CD,
                                            System.String @p_DTY_CD, System.String @p_JB_CD, System.String @p_DISP_YN,
                                            System.String @p_DISP_STR_DT, System.String @p_DISP_END_DT, System.String @p_DOC_NO,
                                            System.String @p_RMKS, System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_PAPP_DT,
					@p_PAPP_SO,
					@p_PAPP_CORP_CD,
					@p_GCLS_CD,
					@p_SCLS_CD,
					@p_PRE_PAPP_DEPT_CD,
					@p_DEPT_CD,
					@p_PAYSTEP_CD,
					@p_GRADE_CD,
					@p_DTY_CD,
					@p_JB_CD,
					@p_DISP_YN,
					@p_DISP_STR_DT,
					@p_DISP_END_DT,
					@p_DOC_NO,
					@p_RMKS,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }


        public class SHRA_BI_PRIZE_Q : BaseProcClass
        {
            public SHRA_BI_PRIZE_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_PRIZE_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_PRIZE_S : BaseProcClass
        {
            public SHRA_BI_PRIZE_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_PRIZE_S";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RNPNSH_DT", "Varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RNPNSH_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RSN", "Varchar", 60, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RSLT", "Varchar", 40, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.String @p_RNPNSH_DT, System.String @p_RNPNSH_CD, System.String @p_RSN,
                                        System.String @p_RSLT, System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_RNPNSH_DT,
					@p_RNPNSH_CD,
					@p_RSN,
					@p_RSLT,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_SCHCAREER_Q : BaseProcClass
        {
            public SHRA_BI_SCHCAREER_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_SCHCAREER_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO)
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
					@p_EMP_NO
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        public class SHRA_BI_SCHCAREER_S : BaseProcClass
        {
            public SHRA_BI_SCHCAREER_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRA_BI_SCHCAREER_S";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EDUCRR_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_WHSHOL_STR_DT", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_WHSHOL_END_DT", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_SCHL_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_SCHL_NM", "Varchar", 40, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_LPLC_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_MJR_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_SUB_MJR_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_ACDGR_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_RMKS", "Varchar", 200, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.String @p_EDUCRR_CD, System.String @p_WHSHOL_STR_DT, System.String @p_WHSHOL_END_DT,
                                        System.String @p_SCHL_CD, System.String @p_SCHL_NM, System.String @p_LPLC_CD,
                                        System.String @p_MJR_CD, System.String @p_SUB_MJR_CD, System.String @p_ACDGR_CD,
                                        System.String @p_RMKS, System.String @p_USER_ID)
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
					@p_EMP_NO,
					@p_EDUCRR_CD,
					@p_WHSHOL_STR_DT,
					@p_WHSHOL_END_DT,
					@p_SCHL_CD,
					@p_SCHL_NM,
					@p_LPLC_CD,
					@p_MJR_CD,
					@p_SUB_MJR_CD,
					@p_ACDGR_CD,
					@p_RMKS,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        #endregion [End Procedure Information Class]

        #region [panCollapseEvent]
        private void panCollapseList_Click(object sender, EventArgs e)
        {
            panList.Visible = !(panList.Visible);
            splitV.Visible = !(splitV.Visible);
            if (panList.Visible == true)
                panCollapseList.ContentImage = Properties.Resources.Left;
            else
                panCollapseList.ContentImage = Properties.Resources.Right;
        }
        private void panCollapseInfo1_Click(object sender, EventArgs e)
        {
            panInfo1.Visible = !(panInfo1.Visible);
            if (panInfo1.Visible == true)
                panCollapseInfo1.ContentImage = Properties.Resources.Up;
            else
                panCollapseInfo1.ContentImage = Properties.Resources.Down;

        }

        #endregion [End Procedure Information Class]

        #region [TabPage Event]

        //when  Tabpage SelectChanged
        private void tabBaseInfo_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            foreach (Control objItem in ((Control)e.Page).Controls)   //판넬에 속한 모든 판넬의 Control 검색
            {
                if (objItem is PanelEx) //Pannel에 속한 Grid를 찾아서 Select Gird, Select view를 Setting 해준다 
                {
                    foreach (object Gitem in ((Control)objItem).Controls)
                    {
                        if (Gitem is GridControlEx)
                        {
                            //그리드 및 View 관련 코딩은 모두 얘내들로 처리한다
                            SelGird = (GridControlEx)Gitem;             // Select Grid             
                            SelView = (GridViewEx)SelGird.MainView;     // Select Grid.MainView
                            TabQuery();
                            return;
                        }
                    }

                }
            }

        }

        // when Tabpage SelectChanging
        private void tabBaseInfo_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            if (SaveButton == true)
            {
                DialogResult dr = SetYesNoMessageBox("수정한 내용이 있습니다.\r\n저장을 하시고 이동하시겠습니까?\r\n(예:저장후 이동, 아니오:이동)");
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    // if (TabSave() == false) e.Cancel = true;
                    e.Cancel = !TabSave();
                }
            }
        }

        //활성탭 자료 조회하기
        private void TabQuery()
        {


            switch (tabBaseInfo.SelectedTabPageIndex)
            {
                case 0: // 상세정보, 겸직정보
                    SetPanelFromGrid(grdBaseInfo, panDetail);
                    fnQRY_SHRA_BI_CONCURRENT_Q("Q");
                    break;
                case 1:     //  주조
                    fnQRY_SHRA_BI_ADDRESS_Q("Q");
                    break;
                case 2:     // 가족
                    fnQRY_SHRA_BI_FAMILY_Q("Q");
                    break;
                case 3:     // 학력       
                    fnQRY_SHRA_BI_SCHCAREER_Q("Q");
                    break;
                case 4:     // 병역, 신체
                    SetPanelFromGrid(grdBaseInfo, panMilitary);
                    fnQRY_SHRA_BI_MILITARYCAREER_Q("Q");
                    fnQRY_SHRA_BI_HEALTH_Q("Q");
                    break;
                case 5:     //경력
                    fnQRY_SHRA_BI_CAREER_Q("Q");
                    break;
                case 6:     //면허
                    fnQRY_SHRA_BI_LICENSE_Q("Q");
                    break;
                case 7:     //발령
                    fnQRY_SHRA_BI_ORDER_Q("Q");
                    break;
                case 8:     //상벌
                    fnQRY_SHRA_BI_PRIZE_Q("Q");
                    break;
                case 9:     //동호회
                    fnQRY_SHRA_BI_ASSOCIATION_Q("Q");
                    break;
                case 10:    //외국어
                    fnQRY_SHRA_BI_LANGUAGE_Q("Q");
                    break;
                case 11:    //교육
                    fnQRY_SHRA_BI_EDU_Q("Q");
                    break;
                default:
                    break;
            }
            EnabledControls(panSearch, true);
        }

        private bool TabSave()
        {

            if (fnSET_SHRA_BASEINFO_S(panInfo1.DataStatus) == false) return false;

            switch (tabBaseInfo.SelectedTabPageIndex)
            {
                case 0: // 상세정보, 겸직정보   -- 저장없음 
                    break;
                case 1:     //  주소
                    if (fnSET_SHRA_BI_ADDRESS_S("") == false) return false;
                    break;
                case 2:     // 가족
                    if (fnSET_SHRA_BI_FAMILY_S("") == false) return false;
                    break;
                case 3:     // 학력       
                    if (fnSET_SHRA_BI_SCHCAREER_S("") == false) return false;
                    break;
                case 4:     // 병역, 신체
                    if (fnSET_SHRA_BI_MILITARYCAREER_S(panMilitary.DataStatus) == false) return false;
                    if (fnSET_SHRA_BI_HEALTH_S("") == false) return false;
                    break;
                case 5:     //경력
                    if (fnSET_SHRA_BI_CAREER_S("") == false) return false;
                    break;
                case 6:     //면허
                    if (fnSET_SHRA_BI_LICENSE_S("") == false) return false;
                    break;
                case 7:     //발령
                    if (fnSET_SHRA_BI_ORDER_S(panOrder.DataStatus) == false) return false;

                    break;
                case 8:     //상벌
                    if (fnSET_SHRA_BI_PRIZE_S("") == false) return false;
                    break;
                case 9:     //동호회
                    if (fnSET_SHRA_BI_ASSOCIATION_S("") == false) return false;
                    break;
                case 10:    //외국어
                    if (fnSET_SHRA_BI_LANGUAGE_S("") == false) return false;
                    break;
                case 11:    //교육
                    if (fnSET_SHRA_BI_EDU_S("") == false) return false;
                    break;
                default:
                    break;

            }


            return true;
        }


        #endregion[TabPage Event]

        #region [Etc Event]

        //판넬자료 세팅 LayoutView 포함 && Panel 안의 모든 판넬에 자료 세팅
        private void SetPanelFromGrid_ALL(GridControlEx grdGrid, object objPanel)
        {
            if (grdGrid.DataSource != null)
            {
                DataTable dtData = ((DataTable)grdGrid.DataSource).Clone();
                DataRow row = dtData.NewRow();
                if (grdGrid.DefaultView is GridViewEx)
                {
                    GridViewEx ex1 = (GridViewEx)grdGrid.DefaultView;
                    if (ex1.FocusedRowHandle < 0)
                    {
                        return;
                    }
                    if (ex1.GetFocusedRow() == null)
                    {
                        ex1.FocusedRowHandle = 0;
                    }
                    if (ex1.RowCount == ex1.FocusedRowHandle)
                    {
                        ex1.FocusedRowHandle = 0;
                    }
                    for (int i = 0; i < ex1.Columns.Count; i++)
                    {
                        if (dtData.Columns.Contains(ex1.Columns[i].FieldName))
                        {
                            row[ex1.Columns[i].FieldName] = ex1.GetRowCellValue(ex1.FocusedRowHandle, ex1.Columns[i].FieldName);
                        }
                    }
                }
                else if (grdGrid.DefaultView is BandedGridViewEx)
                {
                    BandedGridViewEx ex2 = (BandedGridViewEx)grdGrid.DefaultView;
                    if (ex2.FocusedRowHandle < 0)
                    {
                        return;
                    }
                    if (ex2.RowCount == ex2.FocusedRowHandle)
                    {
                        ex2.FocusedRowHandle = 0;
                    }
                    for (int j = 0; j < ex2.Columns.Count; j++)
                    {
                        row[ex2.Columns[j].FieldName] = ex2.GetRowCellValue(ex2.FocusedRowHandle, ex2.Columns[j].FieldName);
                    }
                }
                else if (grdGrid.DefaultView is AdvBandedGridViewEx)
                {
                    AdvBandedGridViewEx ex3 = (AdvBandedGridViewEx)grdGrid.DefaultView;
                    if (ex3.FocusedRowHandle < 0)
                    {
                        return;
                    }
                    if (ex3.RowCount == ex3.FocusedRowHandle)
                    {
                        ex3.FocusedRowHandle = 0;
                    }
                    for (int k = 0; k < ex3.Columns.Count; k++)
                    {
                        row[ex3.Columns[k].FieldName] = ex3.GetRowCellValue(ex3.FocusedRowHandle, ex3.Columns[k].FieldName);
                    }
                }
                else if (grdGrid.DefaultView is DevExpress.XtraGrid.Views.Layout.LayoutView)
                {
                    DevExpress.XtraGrid.Views.Layout.LayoutView ex4 = (DevExpress.XtraGrid.Views.Layout.LayoutView)grdGrid.DefaultView;
                    if (ex4.FocusedRowHandle < 0)
                    {
                        return;
                    }
                    if (ex4.GetFocusedRow() == null)
                    {
                        ex4.FocusedRowHandle = 0;
                    }
                    if (ex4.RowCount == ex4.FocusedRowHandle)
                    {
                        ex4.FocusedRowHandle = 0;
                    }
                    for (int i = 0; i < ex4.Columns.Count; i++)
                    {
                        if (dtData.Columns.Contains(ex4.Columns[i].FieldName))
                        {
                            row[ex4.Columns[i].FieldName] = ex4.GetRowCellValue(ex4.FocusedRowHandle, ex4.Columns[i].FieldName);
                        }
                    }
                }

                dtData.Rows.Add(row);
                foreach (Control objItem in ((Control)objPanel).Controls)   //판넬에 속한 모든 판넬의 Control에 자료입력
                {
                    if (objItem is PanelEx)
                    {
                        //base.InitControls(objItem);              // 자료 초기화
                        this.SetData(objItem, dtData);
                        ((PanelEx)objItem).DataStatus = "Q";
                    }
                    else if (objItem is GroupControlEx)
                    {
                        //base.InitControls(objItem);              // 자료 초기화
                        this.SetData(objItem, dtData);
                        ((GroupControlEx)objItem).DataStatus = "Q";
                    }
                }


            }

        }

        //콘드롤내의 모든 자료를 지운다
        private void InitControls_All(object objPan)
        {
            foreach (object objControl in ((Control)objPan).Controls)
            {
                if (objControl is PanelEx || objControl is GroupControlEx || objControl is TabEx || objControl is TabPageEx)
                {
                    base.InitControls(objPan);
                    this.InitControls_All(objControl);
                }
                else if (objControl is GridControlEx)
                    base.InitControls(objControl);
            }
        }

        //panDetail 내용 수정시PanInfo1 DataStatus 변경 
        private void panDetail_DataStatusChanged(object sender, EventArgs e)
        {
            panInfo1.DataStatus = panDetail.DataStatus;
        }

        //발령대분류에 따라 필수입력값과 선택입력값 입력제어
        private void OrderControl_Setting(string strGclsCD)
        {
            Control[] arrMandantory = null;
            Control[] arrOptional = null;

            switch (strGclsCD)
            {
                case "01": // 채용, 부서이동, 겸직 - 발령부서만 필수
                case "05":
                case "08":
                    arrMandantory = new Control[] { txtDEPT_CD1 };
                    arrOptional = new Control[] { txtJB_CD, txtDTY_CD };
                    break;
                case "02":
                case "03":
                case "04":
                case "12":
                case "16": //휴직,복직,퇴직,대기,징계, 파견해제
                    break;
                case "06": //파견, 파견해지
                case "07":
                case "23":
                    //arrOptional = new Control[] { txtJB_CD, txtDTY_CD };
                    //break;
                case "24":
                    arrMandantory = new Control[] { txtDEPT_CD1 };
                    arrOptional = new Control[] { txtJB_CD, txtDTY_CD };
                    break;
                case "09": //겸직해제
                    arrMandantory = new Control[] { txtDEPT_CD1 };
                    break;
                case "13": //보직부여
                    arrMandantory = new Control[] { txtDEPT_CD1, txtDTY_CD };
                    arrOptional = new Control[] { txtJB_CD };
                    break;
                case "14": //면보직
                    arrMandantory = new Control[] { txtDEPT_CD1, txtDTY_CD };
                    break;
                case "20":
                case "21":
                case "22": //승진, 승급, 승호, 강등
                    arrMandantory = new Control[] { txtPAYSTEP_CD1, txtGRADE_CD1 };
                    arrOptional = new Control[] { txtJB_CD };
                    break;
                case "99":
                    arrOptional = new Control[] { txtJB_CD };
                    break;
            }

            if (arrMandantory != null)
            {
                foreach (Control ctrl in arrMandantory)
                {
                    ((ButtonEditEx)ctrl).Enabled = true;
                    ((ButtonEditEx)ctrl).Properties.ReadOnly = false;
                    ((ButtonEditEx)ctrl).Properties.Tag = "";
                    ((ButtonEditEx)ctrl).Properties.AllowBlank = false;
                    ((ButtonEditEx)ctrl).BackColor = Color.MistyRose;
                }
            }

            if (arrOptional != null)
            {
                foreach (Control ctrl in arrOptional)
                {
                    ((ButtonEditEx)ctrl).Enabled = true;
                    ((ButtonEditEx)ctrl).Properties.ReadOnly = false;
                    ((ButtonEditEx)ctrl).Properties.Tag = "";
                    ((ButtonEditEx)ctrl).Properties.AllowBlank = true;
                }
            }

            ymdPAPP_DT.Properties.ReadOnly = false;
            ymdPAPP_DT.Properties.AllowBlank = false;
            ymdPAPP_DT.Properties.Tag = "";
            ymdPAPP_DT.Enabled = true;

            txtGCLS_CD.Properties.ReadOnly = false;
            txtGCLS_CD.Properties.AllowBlank = false;
            txtGCLS_CD.Properties.Tag = "";
            txtGCLS_CD.Enabled = true;

            txtSCLS_CD.Properties.ReadOnly = false;
            txtSCLS_CD.Properties.AllowBlank = false;
            txtSCLS_CD.Properties.Tag = "";
            txtSCLS_CD.Enabled = true;

            txtRMKS.Properties.ReadOnly = false;
            txtRMKS.Properties.AllowBlank = true;
            txtRMKS.Properties.Tag = "";
            txtRMKS.Enabled = true;

        }

        private void OrderControl_ReadOnly()
        {
            ymdPAPP_DT.Properties.ReadOnly = false;			//	발령일자
            txtDOC_NO.Properties.ReadOnly = false;			// 	문서번호
            txtGCLS_CD.Properties.ReadOnly = false;			// 	발령대분류
            txtSCLS_CD.Properties.ReadOnly = false;			// 	발령소분류
            txtGRADE_CD1.Properties.ReadOnly = true;		// 	직급
            txtPAYSTEP_CD1.Properties.ReadOnly = true;		// 	급호
            txtDTY_CD.Properties.ReadOnly = true;			// 	직책
            txtDEPT_CD1.Properties.ReadOnly = true;			// 	발령부서
            txtJB_CD.Properties.ReadOnly = true;			// 	직무
            ymdPAPP_DT.Properties.Tag = "";
            txtDOC_NO.Properties.Tag = "";
            txtGCLS_CD.Properties.Tag = "";
            txtSCLS_CD.Properties.Tag = "";
            txtGRADE_CD1.Properties.Tag = "9000";
            txtPAYSTEP_CD1.Properties.Tag = "9000";
            txtDTY_CD.Properties.Tag = "9000";
            txtDEPT_CD1.Properties.Tag = "9000";
            txtJB_CD.Properties.Tag = "9000";
            ymdPAPP_DT.Properties.AllowBlank = false;
            txtDOC_NO.Properties.AllowBlank = true;
            txtGCLS_CD.Properties.AllowBlank = false;
            txtSCLS_CD.Properties.AllowBlank = false;
            txtGRADE_CD1.Properties.AllowBlank = true;
            txtPAYSTEP_CD1.Properties.AllowBlank = true;
            txtDTY_CD.Properties.AllowBlank = true;
            txtDEPT_CD1.Properties.AllowBlank = true;
            txtJB_CD.Properties.AllowBlank = true;
            txtGRADE_CD1.Enabled = false;
            txtPAYSTEP_CD1.Enabled = false;
            txtDTY_CD.Enabled = false;
            txtDEPT_CD1.Enabled = false;
            txtJB_CD.Enabled = false;
            txtGRADE_CD1.BackColor = Color.White;
            txtPAYSTEP_CD1.BackColor = Color.White;
            txtDTY_CD.BackColor = Color.White;
            txtDEPT_CD1.BackColor = Color.White;
            txtJB_CD.BackColor = Color.White;
        }

        //발령대분류명칭(코드) 변경됐을때
        private void txtGCLS_CD_Leave(object sender, EventArgs e)
        {
            if (panOrder.DataStatus == "N" || panOrder.DataStatus == "U")
            {
                txtSCLS_CD.Text = "";
                txtSCLS_NM.Text = "";
                txtPAYSTEP_CD1.Text = "";
                txtPAYSTEP_NM1.Text = "";
                txtGRADE_CD1.Text = "";
                txtGRADE_NM1.Text = "";
                txtDTY_CD.Text = "";
                txtDTY_NM.Text = "";
                txtJB_CD.Text = "";
                txtJB_NM.Text = "";
                txtDEPT_CD1.Text = "";
                txtDEPT_NM1.Text = "";
                txtPAPP_CORP_CD.Text = "";

                OrderControl_ReadOnly();
                OrderControl_Setting(txtGCLS_CD.Text);  // 발령대분류에 따른 입력 컨트롤 세팅
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

        //주민 등록 번호 형식 체크
        private bool ChkSocialNo(String strNO)
        {

            String str = ReturnOnlyNumeric(strNO);
            String strFirstNo = "";

            if (strNO == null || strNO == "")
            {
                return false;
            }

            if (str.Length == 13)
            {
                bool chkNumeric = true;
                foreach (Char chr in str)
                {
                    if (Char.IsNumber(chr) == false)
                    {
                        chkNumeric = false;
                        break;
                    }
                }

                if (chkNumeric == true)
                {
                    strFirstNo = str.Substring(6, 1);

                    if (strFirstNo == "1" || strFirstNo == "2" || strFirstNo == "3" || strFirstNo == "4")
                    {
                        int checkNum = 0;
                        int[] no = new int[13];

                        for (int i = 0; i <= 12; i++)
                        {
                            no[i] = int.Parse(str.Substring(i, 1).ToString());
                        }
                        checkNum = 11 - (no[0] * 2 + no[1] * 3 + no[2] * 4 + no[3] * 5 + no[4] * 6 + no[5] * 7 + no[6] * 8 + no[7] * 9 + no[8] * 2 + no[9] * 3 + no[10] * 4 + no[11] * 5) % 11;

                        if (checkNum >= 10 && checkNum <= 11)
                        {
                            checkNum = checkNum - 10;
                        }

                        if (no[12] != checkNum)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }

            return false;

        }

        //주민등록 번호 체크 -- 최초 입력시 한번만 진행하고 저장후에는 체크 하지 않는다
        private void txtRSDN_NO_Leave(object sender, EventArgs e)
        {
            if (!ChkSocialNo(txtRSDN_NO.Text.ToString()))
                SetMessageBox("올바른 주민등록번호가 아닙니다. 확인 후 작업하시기 바랍니다.", IconType.Warning);

            if (txtRSDN_NO.Text.Replace("-", "").Length == 13)
            {
                txtRSDN_NO.Text = ReturnOnlyNumeric(txtRSDN_NO.Text).Substring(0, 6) + "-" + ReturnOnlyNumeric(txtRSDN_NO.Text).Substring(6);
                if (ymdBRTH.EditValue.ToString() == "")
                    ymdBRTH.EditValue = int.Parse(ReturnOnlyNumeric(txtRSDN_NO.Text).Substring(6, 1)) <= 2 ? "19" + txtRSDN_NO.Text.Substring(0, 6) : "20" + txtRSDN_NO.Text.Substring(0, 6);
                radGENDER_TY.EditValue = int.Parse(ReturnOnlyNumeric(txtRSDN_NO.Text).Substring(6, 1)) % 2 == 1 ? "M" : "W";
            }
        }

        //법인코드 세팅
        private void WorkCorpCode(string SetorGet)
        {
            if (SetorGet.ToUpper() == "SET")
            {
                //화면 로딩시 법인코드 세팅
                if (csSessionInfo.HR_SPACE[0] == null || csSessionInfo.HR_SPACE[0] == "")
                {
                    csSessionInfo.HR_SPACE[0] = SessionInfo.CORP_CD.ToString();
                }
                ctxtCORP_CD.Text = csSessionInfo.HR_SPACE[0];
                ctxtCORP_NM.Text = SessionInfo.CORP_NM;
                //CallPopup(ctxtCORP_CD, "", 0, "ValueCheck");
            }
            else if (SetorGet.ToUpper() == "GET")
            {
                //법인코드 변경시 법인코드 메모리 변수 Setting
                if (ctxtCORP_CD.Text != "")
                {
                    csSessionInfo.HR_SPACE[0] = ctxtCORP_CD.Text;
                }
            }
        }

        //법인코드 변경시 메모리 변수에 세팅
        private void ctxtCORP_CD_Leave(object sender, EventArgs e)
        {
            WorkCorpCode("GET");
        }

        #endregion

        #region [우편번호 API]

        private void gvwAddress_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            ZipSearch();
        }

        private void gvwAddress_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            bZpFocusedColumn = true;
            ZipSearch();
            bZpFocusedColumn = false;
        }

        private void gvwAddress_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (gvwAddress.FocusedColumn == colZPCD)
            {
                if (e.Value.ToString().Trim() == "")
                {
                    if (gvwAddress.GetRowCellValue(e.RowHandle, colADDR).ToString().Trim() != "") gvwAddress.SetRowCellValue(e.RowHandle, colADDR, "");
                }
                else
                {
                    if (!bZpFocusedColumn) ZipSearch();
                }
            }
        }

        // 우편번호 적용 Config 설정
        private void ZipCodeConfig()
        {
            ResultSet rs = CommonDirectSQL("SELECT sub_code FROM TCCZ_CodeMaster WHERE group_code = 'SYS004' AND extra_field1 = 'Y'");

            if (rs.ResultDataSet.Tables[0].Rows[0]["sub_code"].ToString() == "2")
            {
                this.colZPCD.Popup.BizComponentID = "";
                this.colZPCD.Popup.PopupEvent = "";

                gvwAddress.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(gvwAddress_RowCellClick);
                gvwAddress.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(gvwAddress_FocusedColumnChanged);
                gvwAddress.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gvwAddress_CellValueChanged);
            }
            else
            {
                gvwAddress.RowCellClick -= new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(gvwAddress_RowCellClick);
                gvwAddress.FocusedColumnChanged -= new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(gvwAddress_FocusedColumnChanged);
                gvwAddress.CellValueChanged -= new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gvwAddress_CellValueChanged);
            }

        }

        // 우편번호 조회
        private void ZipSearch()
        {
            if (gvwMaster.RowCount <= 0)
                return;

            if (gvwAddress.FocusedColumn == colZPCD && gvwAddress.OptionsBehavior.Editable && gvwAddress.FocusedColumn.OptionsColumn.AllowEdit && !gvwAddress.FocusedColumn.OptionsColumn.ReadOnly)
            {
                Hashtable ht = new Hashtable();
                object objResult = OpenChildForm(@"CC\vPlus.erp.CC.Ccz_ZipCode.dll", ht, OpenType.Modal);

                if (objResult is Hashtable)
                {
                    gvwAddress.FocusedColumn = colADDR;
                    ht = (Hashtable)objResult;
                    gvwAddress.SetRowCellValue(gvwAddress.FocusedRowHandle, colZPCD, ht["postcode"].ToString());
                    gvwAddress.SetRowCellValue(gvwAddress.FocusedRowHandle, colADDR, ht["address"].ToString());
                }
            }
        }

        #endregion


        private void cboRSC_DIV_EditValueChanged(object sender, EventArgs e)
        {
            panMilitary.DataStatus = "U";

        }

        private void cboGUNB_CD_EditValueChanged(object sender, EventArgs e)
        {
            panMilitary.DataStatus = "U";
        }

        private void cboBUNGG_CD_EditValueChanged(object sender, EventArgs e)
        {
            panMilitary.DataStatus = "U";
        }

        private void cboGRADE_CD_EditValueChanged(object sender, EventArgs e)
        {
            panMilitary.DataStatus = "U";
        }

        private void ymdGEDE_DT_EditValueChanged(object sender, EventArgs e)
        {
            panMilitary.DataStatus = "U";
        }

        private void gvwSchool_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.OptionsColumn.AllowEdit == true) EnabledControls(panSearch, false);

            //무학~고재 까지 text 그 이상은 popup
            if (e.Column.FieldName == "EDUCRR_CD")
            {
                if (parseInt32(gvwSchool.GetValue("EDUCRR_CD").ToString()) < 40)
                {
                    this.colSCHL_NM.ColumnEdit = null;
                    this.colSCHL_NM.Popup.PopupEvent = "";
                }
                else
                {
                    this.colSCHL_NM.ColumnEdit = this.repositoryItemButtonEditEx4;
                    this.colSCHL_NM.Popup.PopupEvent = "DoubleClick,ValueCheck";
                }
            }
        }

        private void btnImageDel_Click(object sender, EventArgs e)
        {
            //picPHOTO.EditValue = DBNull.Value;
            picPHOTO.EditValue = picTMPIMG.EditValue;
            panInfo1.DataStatus = "U";
            fnSetToolbarButton("INSERT");
        }

        private void EnabledForms(bool enable = false)
        {
            // 상세정보
            EnabledExtraControls(menuStrip, enable);
            EnabledControls(panInfo1, enable);
            //panInfo1.Enabled =  enable;   
            picPHOTO.Properties.ShowMenu = enable;
            btnImage.Enabled = enable;
            btnImageDel.Enabled = enable;

            //EnabledControls(panDetail, enable);

            txtUSERID.Enabled = false;
            txtEMAIL_ADDR.Enabled = false;
            txtDISP_DEPT_NM.Enabled = false;
            chkVETER_YN.Enabled = false;
            chkOBST_YN.Enabled = false;
            ymdOBST_DT.Enabled = false;
            txtOBST_DS.Enabled = false;

            //// 주소
            //EnabledExtraControls(menuStrip_Address, enable);
            //EnabledExtraControls(grdAddress, enable);

            //// 가족
            //EnabledExtraControls(menuStrip_Family, enable);
            //EnabledExtraControls(grdFamilly, enable); --20200319 

            // 학력
            EnabledExtraControls(menuStrip_School, enable);
            EnabledExtraControls(grdSchool, enable);

            // 병역/신체
            EnabledExtraControls(menuStrip__Military, enable);
            EnabledControls(panMilitary, enable);
            EnabledExtraControls(menuStrip_Health, enable);
            EnabledExtraControls(grdHealth, enable);

            // 경력
            EnabledExtraControls(menuStrip_Career, enable);
            EnabledExtraControls(grdCareer, enable);

            // 면허
            EnabledExtraControls(menuStrip_License, enable);
            EnabledExtraControls(grdLicense, enable);

            // 발령
            EnabledExtraControls(menuStrip_Order, enable);
            EnabledControls(panOrder, enable);

            // 상벌
            EnabledExtraControls(menuStrip_Prize, enable);
            EnabledExtraControls(grdPrize, enable);

            // 교육
            EnabledExtraControls(menuStrip_Education, enable);
            EnabledExtraControls(grdEducation, enable);
        }

        public void EnabledExtraControls(object objCtr, bool enabled)
        {
            if (objCtr is MenuStrip)
            {
                MenuStrip menuStrip = (MenuStrip)objCtr;

                foreach (ToolStripMenuItem menuItem in menuStrip.Items)
                {
                    if (menuItem.Tag == null || !menuItem.Tag.ToString().Equals("9000"))
                        menuItem.Enabled = enabled;
                }

            }
            else if (objCtr is BandedGridViewEx)
            {
                BandedGridViewEx grdCtr = (BandedGridViewEx)objCtr;
                grdCtr.OptionsBehavior.Editable = enabled;
            }
            else if (objCtr is GridViewEx)
            {
                GridViewEx grdCtr = (GridViewEx)objCtr;
                grdCtr.OptionsBehavior.Editable = enabled;
            }
        }

        private void txtUSERID_Leave(object sender, EventArgs e)
        {
            if (txtUSERID.Text.Contains("@"))
            {
                SetMessageBox("이메일은 아이디만 입력하십시오.");
                return;
            }

            txtEMAIL_ADDR.Text = txtUSERID.Text + txtEMAIL.Text;
        }

        private void txtSCLS_CD_Enter(object sender, EventArgs e)
        {
            OrderControl_Setting(txtGCLS_CD.Text);
        }
	}
}
