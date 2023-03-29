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

//2023.03.29 깃테스트
namespace vPlus.erp.HR
{
    /// <summary>
    /// [연차관리]
    /// 2013.07.19.서진호
    /// </summary>
    public partial class Hrc_VacationMgt : ERPBaseFormInherit
    {
        #region [전역 변수 선언]

        DataSet ds_Rpt = null; //레포트 데이타를 담을 DataSet

        #endregion  //end [전역 변수 선언]
        
        #region [생성자]
        /// <summary>
        /// 생성자 
        /// </summary>
        public Hrc_VacationMgt()
        {
            InitializeComponent();
        }
        #endregion

        #region [Load]
        /// <summary>
        /// Load 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            fnSetToolbarButton("INIT");
            fnFormInitSetting();    // 화면로드 시 기본값 설정
            //fnSetCtlData();       // 타 화면에서 호출되어 로드될 경우 Parameter 세팅
            

            //WorkFlow 호출
            fnRibbonInit(ribWorkflow);

            EnabledControls(panSearch, true);	// 프리폼(panInfo1, panInfo2…)이 있는 경우는 필수
           
        }
        #endregion

        #region [Button Control]
        /// <summary>
        /// 상단 기본버튼 컨트롤
        /// </summary>
        /// <param name="pType"></param>
        protected void fnSetToolbarButton(string pType)
        {
            bool bQuery = false;
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
                        InitControls(grdMaster);
                        InitControls(panInfo1);
                        InitControls(grdDetail);
                        InitControls(grdUser);
                        break;
                    case "QUERYX":
                        bQuery = true;
                        bNew = true;
                        break;
                    case "QUERY":
                        bQuery = true;
                        bNew = true;
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
                bNew = false;           //리본바 신규버튼 비활성화
                bDelete = false;        //리본바 삭제버튼 비활성화

                QueryButton = bQuery;
                NewButton = bNew;
                AddButton = bAddRow;            // 미사용 버튼
                DeleteRowButton = bDelRow;      // 미사용 버튼
                SaveButton = bSave;
                DeleteButton = bDelete;
                PreviewButton = bPreview;
                PrintButton = bPrint;
            }
        }
        #endregion [Button Control Event]

        #region [Control Setting]

        #region fnFormInitSetting - 화면로드 시 기본값 설정
        /// <summary>
        /// 화면로드 시 기본값 설정
        /// </summary>
        public void fnFormInitSetting()
        {
            cymdBASE_YY.Text = CurrentDate("");        //기준년도를 셋팅함
            //조회 조건 초기 셋팅
            WorkCorpCode("SET");    //법인코드 세팅
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

        #region fnCtlChangeData - panSearch 상 Control(조회조건) 변경 시 Grid 초기화
        /// <summary>
        /// panSearch 상 Control(조회조건) 변경 시 Grid 초기화 
        /// panSearch 상 모든 Control의 EditValueChanged 이벤트에 함수연결
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fnCtlChangeData(object sender, EventArgs e)
        {
            fnSetToolbarButton("INIT");
        }
        #endregion

        #region fnEditChangeData - panInfo 상 Control(데이터) 변경 시 panSearch 상 Control(조회조건) Disabled
        /// <summary>
        /// panInfo 상 Control(데이터) 변경 시 panSearch 상 Control(조회조건) Disabled 
        /// panInfo 상 모든 Control의 TextChanged 이벤트에 함수연결
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fnEditChangeData(object sender, EventArgs e)
        {
            if (panInfo1.DataStatus != "Q")
            {
                EnabledControls(panSearch, false);
            }
        }
        #endregion

        #region fnGridCellValueChanged - Grid 자료변경 시 panSearch 상 Control(조회조건) Disabled
        /// <summary>
        /// Grid 자료변경 시 panSearch 상 Control(조회조건) Disabled 
        /// 그리드뷰의 CellValueChanged 이벤트에 함수연결
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fnGridCellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            switch (((JPlatform.Client.Controls.GridViewEx)(sender)).GridControl.Name)
            {
                case "grdDetail":
                    // 입력 가능한 모든 Column
                    if ((e.Column.FieldName == "DS_WORK") || (e.Column.FieldName == "YN_CURRENT") || (e.Column.FieldName == "NO_TEL") || (e.Column.FieldName == "NO_MOBILE") || (e.Column.FieldName == "DS_EMAIL"))
                    {
                        EnabledControls(panSearch, false);
                    }

                    break;

                default:
                    break;
            }
        }
        #endregion

        #endregion [Control Setting]

        #region [Grid Setting]

        #region gvwMaster_FocusedRowChanged - 마스터 그리드 포커스 이동시 이벤트
        /// <summary>
        /// 마스터 그리드 포커스 이동시 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvwMaster_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
                return;

            FocusedRowChanged();

            gvwMaster.Focus();
        }

        private void FocusedRowChanged()
        {

            // CASE 1. - Master Grid에서 전체 칼럼을 가져오는 경우 (특별한 경우가 아닌 한 이 방식을 택함)
            this.SetPanelFromGrid(this.grdMaster, this.panInfo1);

            // CASE 2. - Master Grid에 일부 칼럼만 가져오는 경우 (Master Grid 선택 시 DB에서 조회)
            fnQRY_SHRC_VACATIONMGT_EMP_Q("Q");
            fnQRY_SHRC_VACATIONMGT_USER_Q("Q");
            fnQRY_SHRC_VACATIONMGT_DETAIL_Q("Q");
            fnQRY_SHRC_MONTHLYVACATION_Q("Q");
        }



        #endregion

        #region gvwMaster_BeforeLeaveRow - Panel 혹은 Grid Detail 입력/수정/저장 중 Grid Master Row 변경 불가
        /// <summary>
        /// Panel 혹은 Grid Detail 입력/수정/저장 중 Grid Master Row 변경 불가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvwMaster_BeforeLeaveRow(object sender, DevExpress.XtraGrid.Views.Base.RowAllowEventArgs e)
        {
            if (SaveButton == true)
            {
                DialogResult dr = SetYesNoMessageBox("수정한 내용이 있습니다.\r\n저장을 하시고 이동하시겠습니까?\r\n(예:저장후 이동, 아니오:이동)");
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    if (fnSET_SHRC_VACATIONMGT_DETAIL_S("") == false)
                    {
                        e.Allow = false;
                    }
                }
            }
        }
        #endregion

        #region Grid Detail 상 기 저장된 KEY 값 수정 불가 / 신규 Row만 입력 가능
        /// <summary>
        /// Grid Detail 상 기 저장된 KEY 값 수정 불가 / 신규 Row만 입력 가능
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvwDetail_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
                return;

            if (gvwDetail.GetValue(gvwDetail.Columns[0].FieldName)== null) // 아무 KEY 값 지정
                return;

            if (gvwDetail.GetDataRow(e.FocusedRowHandle).RowState == DataRowState.Added)
            {
                gvwDetail.Columns["FRVACA_DT"].OptionsColumn.AllowEdit = true;
            }
            else
            {
                gvwDetail.Columns["FRVACA_DT"].OptionsColumn.AllowEdit = false;
            }
        }
        #endregion

        #region gvwDetail_ShowingEditor - 휴가사용내역 디테일 그리드 셀 변경전 이벤트
        /// <summary>
        /// 휴가사용내역 디테일 그리드 셀 변경전 이벤트 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvwDetail_ShowingEditor(object sender, CancelEventArgs e)
        {
        }
        #endregion

        #endregion [Grid Setting]

        #region [Start Button Event Code By UIBuilder]

        #region 조회
        /// <summary>
        /// 조회 
        /// </summary>
		public override void QueryClick()
		{
            //마스터 조회 호출
            int iBeforeHandle = gvwMaster.FocusedRowHandle;
            fnSetToolbarButton("INIT");
           if (!fnQRY_SHRC_VACATIONMGT_MASTER_Q("Q")) return;            
            if (gvwMaster.RowCount > 0)
            {
                fnSetToolbarButton("QUERY");
                if (iBeforeHandle == 0 && gvwMaster.FocusedRowHandle == 0)
                    FocusedRowChanged();
            }
            else
            {
                fnSetToolbarButton("QUERYX");
            }
            EnabledControls(panSearch, true);		// 조회 후 조회조건 Enabled = True
		}
        #endregion

        #region 저장
        /// <summary>
        /// 저장
        /// </summary>
        public override void SaveClick()
		{
            if (TabSave() == false) return;
            string SelEmp = gvwMaster.GetFocusedRowCellValue(colEMP_NO).ToString();
            QueryClick();
            gvwMaster.FocusedRowHandle = GetGridRowIndex_HP(gvwMaster, colEMP_NO.FieldName, SelEmp, 0);
            
            //if (fnSET_SHRC_VACATIONMGT_DETAIL_S("") == true)
            //{
            //    string SelEmp = gvwMaster.GetFocusedRowCellValue(colEMP_NO).ToString();
            //    QueryClick();
            //    gvwMaster.FocusedRowHandle = GetGridRowIndex_HP(gvwMaster, colEMP_NO.FieldName,SelEmp,  0);
            //}
 		}

        private bool TabSave()
        {
            switch (tabDetail.SelectedTabPageIndex)
            {
                case 0:     //  주소
                    if (fnSET_SHRC_VACATIONMGT_DETAIL_S("") == false) return false;
                    break;
                case 1:     // 가족
                    if (fnSET_SHRC_MONTHLYVACATION_S("") == false) return false;
                    break;
               default:
                    break;
            }
             return true;
        }
        #endregion

        #region row 생성
        /// <summary>
        /// row 생성
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubAdd_Click(object sender, EventArgs e)
        {
            switch (tabDetail.SelectedTabPageIndex)
            {
                case 0:
                    GridAddNewRow(grdDetail, gvwDetail.RowCount);
                    break;
                case 1:
                    GridAddNewRow(grdDetail2, gridViewEx3.RowCount);
                    break;
            }

           
        }
        #endregion

        #region row 삭제
        /// <summary>
        /// row 삭제 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubDel_Click(object sender, EventArgs e)
        {

            switch (tabDetail.SelectedTabPageIndex)
            {
                case 0:
                    if (gvwDetail.RowCount > 0)
                    {
                        GridDeleteRow(grdDetail, false);
                    }
                    break;
                case 1:
                    if (gridViewEx3.RowCount > 0)
                    {
                        GridDeleteRow(grdDetail2, false);
                    }
                    break;
            }
            
            
            
        }
        #endregion

        #endregion [End Button Event]

        #region [Other Button Control]

        #region btnCreateYear_Click - 년차 생성 버튼
        /// <summary>
        /// 년차 생성 버튼 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateYear_Click(object sender, EventArgs e)
        {
            fnQRY_SHRU_BASE_YMD_Q("Q");

            Hashtable hashParam = new Hashtable();
            
            if (ds_Rpt.Tables[0].Rows[0]["BASE_YMD"].ToString() != "")
            {
                hashParam.Add("BASE_YMD", cymdBASE_YY.yyyy + ds_Rpt.Tables[0].Rows[0]["BASE_YMD"].ToString());
            }
            else{
                hashParam.Add("BASE_YMD", cymdBASE_YY.yyyy + "-01-01");                
            }
           
            //Hashtable hashParam = new Hashtable();
            hashParam.Add("CORP_CD", ctxtCORP_CD.Text.ToString());
            hashParam.Add("CORP_NM", ctxtCORP_NM.Text.ToString());
            hashParam.Add("BASE_YY", cymdBASE_YY.yyyy);
            //hashParam.Add("BASE_YMD", cymdBASE_YY.yyyy +  "-01-01");
            hashParam.Add("EMP_NO", ctxtEMP_NO.Text.ToString());
            hashParam.Add("EMP_NM", ctxtEMP_NM.Text.ToString());
            
            Hrc_VacationCreateDlg popUP = new Hrc_VacationCreateDlg();

            //*팝업호출시 필수
            popUP._browserMain = this._browserMain;

            //해쉬테이블을 자식창으로 념겨줌.
            popUP.ParentParameter = hashParam;

            if ( popUP.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {

                String SelEmp = popUP.FormResult.ToString();
                QueryClick();
                if (gvwMaster.RowCount >=0 && SelEmp != "")
                    gvwMaster.FocusedRowHandle = GetGridRowIndex_HP(gvwMaster, colEMP_NO.FieldName, SelEmp, 0);

            }

        }




        private void btnCreateMonth_Click(object sender, EventArgs e)
        {

            fnQRY_SHRU_BASE_YMD_Q("Q");

            Hashtable hashParam = new Hashtable();

            if (ds_Rpt.Tables[0].Rows[0]["BASE_YMD"].ToString() != "")
            {
                hashParam.Add("BASE_YMD", cymdBASE_YY.yyyy + ds_Rpt.Tables[0].Rows[0]["BASE_YMD"].ToString());
            }
            else
            {
                hashParam.Add("BASE_YMD", cymdBASE_YY.yyyy + "-01-01");
            }

            //Hashtable hashParam = new Hashtable();
            hashParam.Add("CORP_CD", ctxtCORP_CD.Text.ToString());
            hashParam.Add("CORP_NM", ctxtCORP_NM.Text.ToString());
            //hashParam.Add("BASE_YY", cymdBASE_YY.yyyy);
            //hashParam.Add("BASE_YMD", cymdBASE_YY.yyyy +  "-01-01");
            hashParam.Add("BASE_YY", CurrentDate(""));
            hashParam.Add("EMP_NO", ctxtEMP_NO.Text.ToString());
            hashParam.Add("EMP_NM", ctxtEMP_NM.Text.ToString());

            Hrc_VacationCreateMonthDlg popUP = new Hrc_VacationCreateMonthDlg();

            //*팝업호출시 필수
            popUP._browserMain = this._browserMain;

            //해쉬테이블을 자식창으로 념겨줌.
            popUP.ParentParameter = hashParam;

            if (popUP.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {

                String SelEmp = popUP.FormResult.ToString();
                QueryClick();
                if (gvwMaster.RowCount >= 0 && SelEmp != "")
                    gvwMaster.FocusedRowHandle = GetGridRowIndex_HP(gvwMaster, colEMP_NO.FieldName, SelEmp, 0);

            }
        }
        #endregion

        #region btnCreateTotYear_Click - 공동연차관리로 이동
        /// <summary>
        /// 공동연차관리로 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateTotYear_Click(object sender, EventArgs e)
        {
            Hashtable hashParam = new Hashtable();
            hashParam.Add("BASE_YY", cymdBASE_YY.yyyy);
            hashParam.Add("CORP_CD", ctxtCORP_CD.Text.ToString());
            hashParam.Add("CORP_NM", ctxtCORP_NM.Text.ToString());

            // 탭 형태로 오픈
            object objResult = OpenChildForm("Hrc_CommonVacation", hashParam, OpenType.Tab, false);

        }
        #endregion


        #endregion  //end [Other Button Control]

        #region 사용자 메소드

        #region ValidateDetailGrid - 저장전 값 유효성 검사
        /// <summary>
        /// 저장전 값 유효성 검사
        /// </summary>
        /// <param name="dtSource"></param>
        private bool ValidateDetailGrid(DataTable dtSource)
        {
            foreach (DataRow dr in dtSource.Rows)
            {
                switch (dr["RowStatus"].ToString())
                {
                    case "N":
                    case "U":
                        //if (cymdBASE_YY.yyyy != dr["FRVACA_DT"].ToString().Substring(0, 4).Trim())
                        //{
                        //    SetMessageBox("시작년도는 " + cymdBASE_YY.yyyy + "년으로 입력하세요!");
                        //    return false;
                        //}

                        //if (cymdBASE_YY.yyyy != dr["TOVACA_DT"].ToString().Substring(0, 4).Trim())
                        //{
                        //    SetMessageBox("종료년도는 " + cymdBASE_YY.yyyy + "년으로 입력하세요!");
                        //    return false;
                        //}

                        if (int.Parse(dr["FRVACA_DT"].ToString()) > int.Parse(dr["TOVACA_DT"].ToString()))
                        {
                            SetMessageBox("종료일자가 시작일자 보다 작습니다.");
                            return false;
                        }

                        break;
                }
            }

            return true;
        }
        #endregion

        #region WorkCorpCode - 법인코드 세팅
        /// <summary>
        /// 법인코드 세팅
        /// </summary>
        /// <param name="SetorGet">
        /// 화면 로딩시 법인코드 세팅 : "SET"
        /// 법인코드 변경시 법인코드 메모리 변수 : "GET"
        /// </param>
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
                CallPopup(ctxtCORP_CD, "", 0, "ValueCheck");
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
        #endregion

        #endregion //end 사용자 메소드

        #region [Start DB Related Code By UIBuilder]

        #region fnQRY_SHRC_VACATIONMGT_MASTER_Q - 연차관리 마스터 조회
        /// <summary>
        /// 연차관리 마스터 조회
        /// </summary>
        /// <param name="strWorkType"></param>
        /// <returns></returns>
        private bool fnQRY_SHRC_VACATIONMGT_MASTER_Q(string strWorkType)
        {
            if (!ValidateControls(panSearch))
                return false;

            try
            {
                // 비즈니스 로직 정보
                SHRC_VACATIONMGT_MASTER_Q cProc = new SHRC_VACATIONMGT_MASTER_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData
                                            , strWorkType
                                            , cymdBASE_YY.yyyy
                                            , ctxtCORP_CD.Text
                                            , ctxtEMP_NO.Text
                                            , ctxtDEPT_CD.Text);

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
        #endregion

        #region fnQRY_SHRC_VACATIONMGT_EMP_Q - 유저의 기본정보 조회
        /// <summary>
        /// 유저의 기본정보 조회
        /// </summary>
        /// <param name="strWorkType"></param>
        /// <returns></returns>
        private bool fnQRY_SHRC_VACATIONMGT_EMP_Q(string strWorkType)
        {
            //if (!ValidateControls(panUser))
            //    return false;

            try
            {
                DataRow row = gvwMaster.GetDataRow(gvwMaster.FocusedRowHandle);

                if (row == null) return false;

                // 비즈니스 로직 정보
                SHRC_VACATIONMGT_EMP_Q cProc = new SHRC_VACATIONMGT_EMP_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData
                                            , strWorkType
                                            , row["CORP_CD"].ToString()
                                            , row["EMP_NO"].ToString());

                CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), panInfo1);
                cProc = null;
                return true;
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
                return false;
            }
        }
        #endregion

        #region fnQRY_SHRC_VACATIONMGT_USER_Q - 개인별 연차사용현황
        /// <summary>
        /// 개인별 연차사용현황
        /// </summary>
        /// <param name="strWorkType"></param>
        /// <returns></returns>
        private bool fnQRY_SHRC_VACATIONMGT_USER_Q(string strWorkType)
        {
            try
            {
                DataRow row = gvwMaster.GetDataRow(gvwMaster.FocusedRowHandle);

                if (row == null) return false;

                // 비즈니스 로직 정보
                SHRC_VACATIONMGT_USER_Q cProc = new SHRC_VACATIONMGT_USER_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData
                                            , strWorkType
                                            , cymdBASE_YY.yyyy
                                            , row["CORP_CD"].ToString()
                                            , row["EMP_NO"].ToString());

                CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdUser);
                cProc = null;
                return true;
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
                return false;
            }
        }
        #endregion

        #region fnQRY_SHRC_VACATIONMGT_DETAIL_Q - 휴가사용내용
        /// <summary>
        /// 휴가사용내용
        /// </summary>
        /// <param name="strWorkType"></param>
        /// <returns></returns>
        private bool fnQRY_SHRC_VACATIONMGT_DETAIL_Q(string strWorkType)
        {
            try
            {
                DataRow row = gvwMaster.GetDataRow(gvwMaster.FocusedRowHandle);

                if (row == null) return false;

                // 비즈니스 로직 정보
                SHRC_VACATIONMGT_DETAIL_Q cProc = new SHRC_VACATIONMGT_DETAIL_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData
                                            , strWorkType
                                            , cymdBASE_YY.yyyy
                                            , row["CORP_CD"].ToString()
                                            , row["EMP_NO"].ToString());

                CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdDetail);
                cProc = null;
                return true;
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
                return false;
            }
        }
        #endregion

        #region fnSET_SHRC_VACATIONMGT_DETAIL_S - 휴가사용내역(연차사용관리) 저장
        /// <summary>
        /// 휴가사용내역(연차사용관리) 저장
        /// </summary>
        /// <param name="strWorkType"></param>
        /// <returns></returns>
        private bool fnSET_SHRC_VACATIONMGT_DETAIL_S(string strWorkType)
		{
			try
			{
                if (!ValidateControls(grdDetail)) return false;

				// 비즈니스 로직 정보
                SHRC_VACATIONMGT_DETAIL_S cProc = new SHRC_VACATIONMGT_DETAIL_S();
				DataTable dtData = null;
				DataTable dtSource = BindingData(grdDetail);
				if( dtSource == null)
					return false;

                //프레임웍의 유효성 검사 후 그리드값 유효성 검사
                if (!ValidateDetailGrid(dtSource)) return false;

                foreach (DataRow dr in dtSource.Rows)
                {
                    dtData = cProc.SetParamData(dtData
                                                , dr["RowStatus"].ToString()
                                                , gvwMaster.GetFocusedRowCellValue(colCORP_CD).ToString()                   //법인코드
                                                , txtEMP_NO.Text                //사번
                                                , dr["FRVACA_DT"].ToString()    //연차사용시작일
                                                , dr["TOVACA_DT"].ToString()    //연차사용종료일
                                                , Decimal.Parse(dr["VACA_QN"].ToString())   //사용일수
                                                , dr["VACA_CD"].ToString()      //연차코드
                                                , dr["REMAK_DS"].ToString()     //비고
                                                , dr["CLOSED_YN"].ToString()
                                                , dr["ELAP_NO"].ToString()			
                                                , dr["ELAP_PRGS_STATS"].ToString()
                                                , dr["APRV_CMPL_DT"].ToString()
                                                , dr["APRV_PSN_NM"].ToString()
                                                , SessionInfo.UserID);

                }

				bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(),grdDetail);
				cProc = null;
				return bResult;
			}
			catch (Exception ex)
			{
				SetErrorMessage(ex);
				return false;
			}
		}

        
        #endregion

     

        //월차사용현황
        private bool fnQRY_SHRC_MONTHLYVACATION_Q(string strWorkType)
        {
            try
            {
                // 비즈니스 로직 정보
                SHRC_MONTHLYVACATION_Q cProc = new SHRC_MONTHLYVACATION_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData,
                                            strWorkType,
                                            gvwMaster.GetValue(gvwMaster.FocusedRowHandle, "CORP_CD").ToString(),
                                            gvwMaster.GetValue(gvwMaster.FocusedRowHandle, "EMP_NO").ToString(),
                                            gvwMaster.GetValue(gvwMaster.FocusedRowHandle, "BASE_YY").ToString()
                                          );

                CommonProcessQuery(dtData, cProc.ProcName, cProc.GetParamInfo(), grdDetail2);
                cProc = null;
                return true;
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
                return false;
            }
        }

        private bool fnSET_SHRC_MONTHLYVACATION_S(string strWorkType)
        {
            try
            {
                // 비즈니스 로직 정보
                SHRC_MONTHLYVACATION_S cProc = new SHRC_MONTHLYVACATION_S();
                DataTable dtData = null;
                DataTable dtSource = BindingData(grdDetail2);
                if (dtSource == null)
                    return false;

                foreach (DataRow dr in dtSource.Rows)
                {
                    dtData = cProc.SetParamData(dtData,
                                    dr["RowStatus"].ToString(),
                                    gvwMaster.GetValue(gvwMaster.FocusedRowHandle, "CORP_CD").ToString(),
                                    gvwMaster.GetValue(gvwMaster.FocusedRowHandle, "EMP_NO").ToString(),
                                    dr["BASE_YMD"].ToString().Replace("-", ""),
                                    Decimal.Parse(dr["MONTHVACA_QN"].ToString()),
                                    dr["USE_YN"].ToString(),
                                    SessionInfo.UserID);

                }
                bool bResult = CommonProcessSave(dtData, cProc.ProcName, cProc.GetParamInfo(), grdDetail);
                cProc = null;
                return bResult;
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
                return false;
            }
        }

        #region fnQRY_SHRU_BASE_YMD_Q - 기준일 조회 조회
        /// <summary>
        /// 연차수당정보 조회 조회
        /// </summary>
        /// <param name="strWorkType"></param>
        /// <returns></returns>
        private bool fnQRY_SHRU_BASE_YMD_Q(string strWorkType)
        {
            if (!ValidateControls(panSearch))
                return false;

            try
            {
                // 비즈니스 로직 정보
                SHRU_BASE_YMD_Q cProc = new SHRU_BASE_YMD_Q();
                DataTable dtData = null;
                dtData = cProc.SetParamData(dtData,
                                strWorkType);

                ResultSet rs = CommonCallQuery(dtData, cProc.ProcName, cProc.GetParamInfo());
                
                ds_Rpt = rs.ResultDataSet;

                cProc = null;
                return true;
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex);
                return false;
            }
        }
        #endregion

        #endregion [End DB Related Code]

        #region [Procedure Information Class By UIBuilder]

        #region Class 마스터 조회 Biz
        /// <summary>
        /// 마스터 조회 Biz
        /// </summary>
        public class SHRC_VACATIONMGT_MASTER_Q : BaseProcClass
        {
            public SHRC_VACATIONMGT_MASTER_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRC_VACATIONMGT_MASTER_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_BASE_YY", "Varchar", 4, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_DEPT_CD", "Varchar", 10, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable
                                         , System.String @p_work_type
                                         , System.String @p_BASE_YY
                                         , System.String @p_CORP_CD
                                         , System.String @p_EMP_NO
                                         , System.String @p_DEPT_CD)
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
					@p_BASE_YY,
					@p_CORP_CD,
					@p_EMP_NO,
					@p_DEPT_CD
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }

        #endregion //end Class 마스터 조회 Biz

        #region Class 기본정보 조회 Biz
        /// <summary>
        /// 기본정보 조회 Biz
        /// </summary>
        public class SHRC_VACATIONMGT_EMP_Q : BaseProcClass
        {
            public SHRC_VACATIONMGT_EMP_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRC_VACATIONMGT_EMP_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable
                                          , System.String @p_work_type
                                          , System.String @p_CORP_CD
                                          , System.String @p_EMP_NO)
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

        #endregion  //end Class 기본정보 조회 Biz

        #region Class 개인별 연차사용현황 Biz
        /// <summary>
        /// Class 개인별 연차사용현황 Biz 
        /// </summary>
        public class SHRC_VACATIONMGT_USER_Q : BaseProcClass
        {
            public SHRC_VACATIONMGT_USER_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRC_VACATIONMGT_USER_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_BASE_YY", "Varchar", 4, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_BASE_YY, System.String @p_CORP_CD,
                                        System.String @p_EMP_NO)
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
					@p_BASE_YY,
					@p_CORP_CD,
					@p_EMP_NO
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }
        #endregion // end Class 개인별 연차사용현황 Biz

        #region Class 휴가사용내역(연차사용관리) 조회
        /// <summary>
        /// Class 휴가사용내역(연차사용관리) 조회
		/// </summary>
		public class SHRC_VACATIONMGT_DETAIL_Q : BaseProcClass
        {
            #region 
            /// <summary>
            /// 생성자
            /// </summary>
            public SHRC_VACATIONMGT_DETAIL_Q()
			{
				// Modify Code : Procedure Name
				_ProcName = "SHRC_VACATIONMGT_DETAIL_Q";
				ParamAdd();
			}
            #endregion

            private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
					_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_BASE_YY", "Varchar", 4, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input",typeof(System.String )));
			}

			public DataTable SetParamData(DataTable dataTable,System.String @p_work_type,System.String @p_BASE_YY,System.String @p_CORP_CD,
										System.String @p_EMP_NO)
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
					@p_BASE_YY,
					@p_CORP_CD,
					@p_EMP_NO
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

        #endregion  //end Class 휴가사용내역(연차사용관리) 조회

        #region Class 휴가사용내역(연차사용관리) 저장
        /// <summary>
        /// Class 휴가사용내역(연차사용관리) 저장
        /// </summary>
        public class SHRC_VACATIONMGT_DETAIL_S : BaseProcClass
		{
            public SHRC_VACATIONMGT_DETAIL_S()
			{
				// Modify Code : Procedure Name
                _ProcName = "SHRC_VACATIONMGT_DETAIL_S";
				ParamAdd();
			}

			private void ParamAdd()
			{
				// Modify Code : Procedure Parameter
					_ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_FRVACA_DT", "Varchar", 8, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_TOVACA_DT", "Varchar", 8, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_VACA_QN", "Decimal", 9, "Input",typeof(System.Decimal )));
					_ParamInfo.Add(new ParamInfo("@p_VACA_CD", "Varchar", 6, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_REMAK_DS", "Varchar", 1000, "Input",typeof(System.String )));
					_ParamInfo.Add(new ParamInfo("@p_CLOSED_YN", "Varchar", 1, "Input",typeof(System.String )));
                    _ParamInfo.Add(new ParamInfo("@p_ELAP_NO", "Varchar", 50, "Input", typeof(System.String)));
                    _ParamInfo.Add(new ParamInfo("@p_ELAP_PRGS_STATS", "Varchar", 10, "Input", typeof(System.String)));
                    _ParamInfo.Add(new ParamInfo("@p_APRV_CMPL_DT", "Varchar", 8, "Input", typeof(System.String)));
                    _ParamInfo.Add(new ParamInfo("@p_APRV_PSN", "Varchar", 15, "Input", typeof(System.String)));
					_ParamInfo.Add(new ParamInfo("@p_USER_ID", "Varchar", 30, "Input",typeof(System.String )));
			}

			public DataTable SetParamData(DataTable dataTable,System.String @p_work_type,System.String @p_CORP_CD,System.String @p_EMP_NO,
										System.String @p_FRVACA_DT,System.String @p_TOVACA_DT,System.Decimal @p_VACA_QN,
										System.String @p_VACA_CD,System.String @p_REMAK_DS, System.String @p_CLOSED_YN,
                                        System.String @p_ELAP_NO, System.String @p_ELAP_PRGS_STATS, System.String @p_APRV_CMPL_DT, System.String @p_APRV_PSN, 
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
					@p_FRVACA_DT,
					@p_TOVACA_DT,
					@p_VACA_QN,
					@p_VACA_CD,
					@p_REMAK_DS,
					@p_CLOSED_YN,
                    @p_ELAP_NO,
                    @p_ELAP_PRGS_STATS,
                    @p_APRV_CMPL_DT,
                    @p_APRV_PSN,
					@p_USER_ID
				};
				dataTable.Rows.Add(objData);
				return dataTable;
			}
		}

        public class SHRC_MONTHLYVACATION_S : BaseProcClass
        {
            public SHRC_MONTHLYVACATION_S()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRC_MONTHLYVACATION_S";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_BASE_YMD", "Varchar", 8, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_MONTHVACA_QN", "Decimal", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USE_YN", "Varchar", 1, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_USER_ID", "varchar", 15, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.String @p_BASE_YMD, System.Decimal @p_MONTHVACA_QN, System.String @p_USE_YN, System.String @p_USER_ID)
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
		            @p_BASE_YMD,        
                    @p_MONTHVACA_QN,
                    @p_USE_YN,
					@p_USER_ID
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }
        #endregion

        #region class 기준일 조회 조회 Biz
        /// <summary>
        /// class 연차수당정보 조회 조회 Biz
        /// </summary>
        public class SHRU_BASE_YMD_Q : BaseProcClass
        {
            public SHRU_BASE_YMD_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRU_BASE_YMD_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type)
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
					@p_work_type
				};
                dataTable.Rows.Add(objData);
                return dataTable;
            }
        }
        #endregion  //end class 기준일 조회 Biz

        public class SHRC_MONTHLYVACATION_Q : BaseProcClass
        {
            public SHRC_MONTHLYVACATION_Q()
            {
                // Modify Code : Procedure Name
                _ProcName = "SHRC_MONTHLYVACATION_Q";
                ParamAdd();
            }

            private void ParamAdd()
            {
                // Modify Code : Procedure Parameter
                _ParamInfo.Add(new ParamInfo("@p_work_type", "Varchar", 10, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_CORP_CD", "Varchar", 6, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_EMP_NO", "Varchar", 15, "Input", typeof(System.String)));
                _ParamInfo.Add(new ParamInfo("@p_BASE_YM", "Varchar", 4, "Input", typeof(System.String)));
            }

            public DataTable SetParamData(DataTable dataTable, System.String @p_work_type, System.String @p_CORP_CD, System.String @p_EMP_NO,
                                        System.String @p_BASE_YY)
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
					@p_BASE_YY
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

        private void ctxtCORP_CD_Leave(object sender, EventArgs e)
        {
            WorkCorpCode("GET"); 
        }

        private void tabDetail_SelectedPageChanging(object sender, DevExpress.XtraTab.TabPageChangingEventArgs e)
        {
            if (SaveButton == true)
            {
                DialogResult dr = SetYesNoMessageBox("수정한 내용이 있습니다.\r\n저장을 하시고 이동하시겠습니까?\r\n(예:저장후 이동, 아니오:이동)");
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = !TabSave();
                }
                else
                {
                    
                }

            }
        }

        private void tabDetail_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            fnQRY_SHRC_VACATIONMGT_DETAIL_Q("Q");
            fnQRY_SHRC_MONTHLYVACATION_Q("Q");
        }


       

       

    }
}
