﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BizOneShot.Light.Models.DareModels;
using BizOneShot.Light.Models.ViewModels;
using BizOneShot.Light.Models.WebModels;
using System.Data.SqlClient;

namespace BizOneShot.Light.Web.ComLib
{
    public static class ReportHelper
    {
        public static SelectList MakeBizWorkList(IList<ScBizWork> scBizWorkList)
        {
            var bizWorkList = new List<SelectListItem>();
            bizWorkList.Add(new SelectListItem { Value = "0", Text = "사업명 선택", Selected = true });

            if (scBizWorkList != null)
            {
                foreach (var item in scBizWorkList)
                {
                    bizWorkList.Add(new SelectListItem { Value = item.BizWorkSn.ToString(), Text = item.BizWorkNm });
                }
            }

            SelectList list = new SelectList(bizWorkList, "Value", "Text");
            return list;
        }

        public static SelectList MakeCompanyList(IList<ScCompInfo> scCompInfoList)
        {
            var companyList = new List<SelectListItem>();
            companyList.Add(new SelectListItem { Value = "0", Text = "기업명 선택", Selected = true });

            if (scCompInfoList != null)
            {
                foreach (var item in scCompInfoList)
                {
                    companyList.Add(new SelectListItem { Value = item.CompSn.ToString(), Text = item.CompNm });
                }
            }

            SelectList list = new SelectList(companyList, "Value", "Text");
            return list;
        }

        public static SelectList MakeReportStatusList()
        {
            var statusList = new List<SelectListItem>();
            statusList.Add(new SelectListItem { Value = "", Text = "작성상태 선택", Selected = true });
            statusList.Add(new SelectListItem { Value = "T", Text = "미작성", Selected = true });
            statusList.Add(new SelectListItem { Value = "W", Text = "작성중", Selected = true });
            statusList.Add(new SelectListItem { Value = "C", Text = "작성완료", Selected = true });
            SelectList list = new SelectList(statusList, "Value", "Text");
            return list;
        }
        /// <summary>
        /// 사업정보를 이용하여 사업의 시작년 부터 종료년 까지 년도 리스트 생성
        /// </summary>
        /// <param name="scBizWork"></param>
        /// <returns></returns>
        public static SelectList MakeBizYear(ScBizWork scBizWork)
        {
            //사업년도
            var year = new List<SelectListItem>();

            year.Add(new SelectListItem { Value = "0", Text = "년도선택", Selected = true });

            if (scBizWork != null)
            {
                for (int i = scBizWork.BizWorkStDt.GetValueOrDefault().Year; i <= scBizWork.BizWorkEdDt.GetValueOrDefault().Year; i++)
                {
                    if (i > DateTime.Now.Year)
                    {
                        break;
                    }
                    year.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
                }
            }

            SelectList yearList = new SelectList(year, "Value", "Text");

            return yearList;
        }

        public static SelectList MakeYear(int startYear)
        {
            //사업년도
            var year = new List<SelectListItem>();
            year.Add(new SelectListItem { Value = "0", Text = "년도선택", Selected = true });

            for (int i = DateTime.Now.Year; i >= startYear; i--)
            {
                year.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() + "년" });
            }

            SelectList yearList = new SelectList(year, "Value", "Text");

            return yearList;
        }

        public static SelectList MakeBasicSurveyYear(IList<RptMaster> rptMasterList, int selectedYear)
        {
            //기초역량 레포트 년도
            var year = new List<SelectListItem>();

            foreach (var item in rptMasterList)
            {
                bool selected = false;
                if (item.BasicYear == selectedYear)
                    selected = true;

                year.Add(new SelectListItem { Value = item.BasicYear.ToString(), Text = item.BasicYear.ToString() + "년", Selected = selected });
            }

            SelectList yearList = new SelectList(year, "Value", "Text");

            return yearList;
        }


        /// <summary>
        /// 사업정보를 이용하여 특정년도의 유효한 월을 생성
        /// </summary>
        /// <param name="scBizWork"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static SelectList MakeBizMonth(ScBizWork scBizWork, int year = 0)
        {
            //사업년도 범위의 월
            var momth = new List<SelectListItem>();

            momth.Add(new SelectListItem { Value = "0", Text = "월선택", Selected = true });

            if (year > DateTime.Now.Year || scBizWork == null || year > scBizWork.BizWorkEdDt.GetValueOrDefault().Year)
            {
                return new SelectList(momth, "Value", "Text");
            }

            //사업시작년과 종료년이 같을경우
            if (scBizWork.BizWorkStDt.GetValueOrDefault().Year == scBizWork.BizWorkEdDt.GetValueOrDefault().Year)
            {
                for (int i = scBizWork.BizWorkStDt.GetValueOrDefault().Month; i <= scBizWork.BizWorkEdDt.GetValueOrDefault().Month; i++)
                {
                    if (year == DateTime.Now.Year && i == DateTime.Now.Month)
                    {
                        break;
                    }
                    momth.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() + "월" });
                }
                return new SelectList(momth, "Value", "Text");
            }

            //사업시작년이 선택년과 같을경우
            if (year == scBizWork.BizWorkStDt.GetValueOrDefault().Year)
            {
                for (int i = scBizWork.BizWorkStDt.GetValueOrDefault().Month; i <= 12; i++)
                {
                    if (year == DateTime.Now.Year && i == DateTime.Now.Month)
                    {
                        break;
                    }
                    momth.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() + "월" });
                }
                return new SelectList(momth, "Value", "Text");
            }

            //사업 종료년이 선택년과 같을경우
            if (year == scBizWork.BizWorkEdDt.GetValueOrDefault().Year)
            {
                for (int i = 1; i <= scBizWork.BizWorkEdDt.GetValueOrDefault().Month; i++)
                {
                    if (year == DateTime.Now.Year && i == DateTime.Now.Month)
                    {
                        break;
                    }
                    momth.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() + "월" });
                }
                return new SelectList(momth, "Value", "Text");
            }

            //선택한 년도가 사업시작년도와 종료년도 사이에 있을경우
            for (int i = 1; i <= 12; i++)
            {
                if (year == DateTime.Now.Year && i == DateTime.Now.Month)
                {
                    break;
                }
                momth.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() + "월" });
            }
            return new SelectList(momth, "Value", "Text");
        }

        public static SelectList MakeMonth(int year = 0)
        {
            //사업년도 범위의 월
            var momth = new List<SelectListItem>();

            momth.Add(new SelectListItem { Value = "0", Text = "월선택", Selected = true });

            if (year == 0)
            {
                return new SelectList(momth, "Value", "Text");
            }


            //과거 년도 선택
            if (year < DateTime.Now.Year)
            {
                for (int i = 1; i <= 12; i++)
                {
                    momth.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() + "월" });
                }
                return new SelectList(momth, "Value", "Text");
            }

            //현재 년도 선택
            for (int i = 1; i < DateTime.Now.Month; i++)
            {
                momth.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() + "월" });
            }
            return new SelectList(momth, "Value", "Text");

        }

        public static SelectList MakeBizQuarter(ScBizWork scBizWork, int year = 0)
        {
            //사업년도 범위의 월
            var quarter = new List<SelectListItem>();

            quarter.Add(new SelectListItem { Value = "0", Text = "분기선택", Selected = true });

            if (year > DateTime.Now.Year || scBizWork == null)
            {
                return new SelectList(quarter, "Value", "Text");
            }

            //사업시작년과 종료년이 같을경우
            if (scBizWork.BizWorkStDt.GetValueOrDefault().Year == scBizWork.BizWorkEdDt.GetValueOrDefault().Year)
            {
                for (int i = scBizWork.BizWorkStDt.GetValueOrDefault().Month; i <= scBizWork.BizWorkEdDt.GetValueOrDefault().Month; i++)
                {
                    int tempQuarter = 0;
                    if (year == DateTime.Now.Year && i == DateTime.Now.Month)
                    {
                        break;
                    }

                    int temp = ((i - 1) / 3) + 1;
                    if (tempQuarter < temp)
                    {
                        tempQuarter = temp;
                        quarter.Add(new SelectListItem { Value = temp.ToString(), Text = temp.ToString() + "분기" });
                    }
                }
                return new SelectList(quarter, "Value", "Text");
            }

            //사업시작년이 선택년과 같을경우
            if (year == scBizWork.BizWorkStDt.GetValueOrDefault().Year)
            {
                int tempQuarter = 0;
                for (int i = scBizWork.BizWorkStDt.GetValueOrDefault().Month; i <= 12; i++)
                {
                    if (year == DateTime.Now.Year && i == DateTime.Now.Month)
                    {
                        break;
                    }
                    int temp = ((i - 1) / 3) + 1;
                    if (tempQuarter < temp)
                    {
                        tempQuarter = temp;
                        quarter.Add(new SelectListItem { Value = temp.ToString(), Text = temp.ToString() + "분기" });
                    }
                }
                return new SelectList(quarter, "Value", "Text");
            }

            //사업 종료년이 선택년과 같을경우
            if (year == scBizWork.BizWorkEdDt.GetValueOrDefault().Year)
            {
                int tempQuarter = 0;
                for (int i = 1; i <= scBizWork.BizWorkStDt.GetValueOrDefault().Month; i++)
                {
                    if (year == DateTime.Now.Year && i == DateTime.Now.Month)
                    {
                        break;
                    }
                    int temp = ((i - 1) / 3) + 1;
                    if (tempQuarter < temp)
                    {
                        tempQuarter = temp;
                        quarter.Add(new SelectListItem { Value = temp.ToString(), Text = temp.ToString() + "분기" });
                    }
                }
                return new SelectList(quarter, "Value", "Text");
            }

            //선택한 년도가 사업시작년도와 종료년도 사이에 있을경우
            if (true)
            {
                int tempQuarter = 0;
                for (int i = 1; i <= 12; i++)
                {

                    if (year == DateTime.Now.Year && i == DateTime.Now.Month)
                    {
                        break;
                    }
                    int temp = ((i - 1) / 3) + 1;
                    if (tempQuarter < temp)
                    {
                        tempQuarter = temp;
                        quarter.Add(new SelectListItem { Value = temp.ToString(), Text = temp.ToString() + "분기" });
                    }
                }
                return new SelectList(quarter, "Value", "Text");
            }
        }

        //Cash Model 생성
        public static CashViewModel MakeCashViewModel(IList<SHUSER_SboBosLiteMonthlyCashReturnModel> cashList)
        {
            CashViewModel cashViewModel = new CashViewModel();

            cashViewModel.ForwardAmt = string.Format("{0:n0}", Convert.ToInt64((cashList[0].LAST_MONTH_CASH / 1000)));   //이월액
            cashViewModel.LastMonthCashBalance = string.Format("{0:n0}", Convert.ToInt64((cashList[0].LAST_MONTH_CASH / 1000))); //전월잔고
            cashViewModel.CashBalance = string.Format("{0:n0}", Convert.ToInt64((cashList[0].CUR_MONTH_CASH / 1000))); //현재잔고
            cashViewModel.ReceivedAmt = string.Format("{0:n0}", Convert.ToInt64((cashList[0].INPUT_AMT / 1000))); //입금액
            cashViewModel.ContributionAmt = string.Format("{0:n0}", Convert.ToInt64((cashList[0].OUTPUT_AMT / 1000))); //출급액

            //var qm = CalcBeforQuarter(int.Parse(cashList[0].ACC_YEAR), int.Parse(cashList[0].ACC_MONTH));

            //Int64 avrBeforQuarter = 0;
            //int cnt = 0;
            //foreach (var cash in cashList)
            //{
            //    if (int.Parse(cash.ACC_YEAR) == qm.Year && (int.Parse(cash.ACC_MONTH) >= qm.Quarter * 3 - 2 && int.Parse(cash.ACC_MONTH) <= qm.Quarter * 3))
            //    {
            //        avrBeforQuarter = avrBeforQuarter + Convert.ToInt64(cash.LAST_AMT);
            //        cnt++;
            //    }
            //}

            cashViewModel.BeforeQuarterlyCashBalance = string.Format("{0:n0}", Convert.ToInt64((cashList[0].LAST_QUARTER_CASH_AVG / 1000))); //전분기
            return cashViewModel;
        }


        //Sales Model 생성
        public static SalesViewModel MakeSalesViewModel(IList<SHUSER_SboMonthlySalesSelectReturnModel> slaesList, SHUSER_SboMonthlyYearSalesSelectReturnModel yearTotal)
        {
            SalesViewModel salesViewModel = new SalesViewModel();

            salesViewModel.CurMonth = string.Format("{0:n0}", Convert.ToInt64((slaesList[0].SALES_AMT / 1000)));   //현월매출
            salesViewModel.LastMonth = string.Format("{0:n0}", Convert.ToInt64((slaesList[1].SALES_AMT / 1000))); //전월매출

            if (yearTotal == null)
            {
                salesViewModel.CurYear = "0";
            }
            else
            {
                salesViewModel.CurYear = string.Format("{0:n0}", Convert.ToInt64((yearTotal.SALES_AMT / 1000))); // 누적매출
            }
            return salesViewModel;
        }

        //이익분석 Model 생성
        public static TotalCostViewModel MakeCostAnalysisViewModel(SHUSER_SboMonthlyCostAnalysisSelectReturnModel cost)
        {
            TotalCostViewModel totalCostViewModel = new TotalCostViewModel();
            totalCostViewModel.AllOtherAmt = string.Format("{0:n0}", Convert.ToInt64((cost.ALL_OTHER_AMT / 1000)));   //영업외비용
            totalCostViewModel.ManufacturingAmt = string.Format("{0:n0}", Convert.ToInt64((cost.MANUFACTURING_AMT / 1000)));   //제조비
            totalCostViewModel.MaterialAmt = string.Format("{0:n0}", Convert.ToInt64((cost.MATERIALS_AMT / 1000)));   //자재비
            totalCostViewModel.OperatingAmt = string.Format("{0:n0}", Convert.ToInt64((cost.OPERATING_AMT / 1000)));   //판관비
            totalCostViewModel.ProfitAmt = string.Format("{0:n0}", Convert.ToInt64((cost.PROFIT_AMT / 1000)));   //이익
            totalCostViewModel.SalesAmt = string.Format("{0:n0}", Convert.ToInt64((cost.SALES_AMT / 1000)));   //매출액    

            return totalCostViewModel;
        }

        //비용분석 Model 생성
        public static ExpenseCostViewModel MakeExpenseCostViewModel(SHUSER_SboMonthlyExpenseCostSelectReturnModel expenseCost)
        {
            ExpenseCostViewModel expenseCostViewModel = new ExpenseCostViewModel();
            expenseCostViewModel.ManAmt = string.Format("{0:n0}", Convert.ToInt64((expenseCost.MAN_AMT / 1000)));   //인건비
            expenseCostViewModel.SalesAmt = string.Format("{0:n0}", Convert.ToInt64((expenseCost.SALES_AMT / 1000)));   //지급 임차료
            expenseCostViewModel.StaticEtcAmt = string.Format("{0:n0}", Convert.ToInt64((expenseCost.STATIC_ETC_AMT / 1000)));   //이자비용
            expenseCostViewModel.WelfareAmt = string.Format("{0:n0}", Convert.ToInt64((expenseCost.WELFARE_AMT / 1000)));   //복리후생비
            expenseCostViewModel.TaxAmt = string.Format("{0:n0}", Convert.ToInt64((expenseCost.TAX_AMT / 1000)));   //세금공과
            expenseCostViewModel.WasteAmt = string.Format("{0:n0}", Convert.ToInt64((expenseCost.WASTE_AMT / 1000)));   //소모품비
            expenseCostViewModel.FloatEtcAmt = string.Format("{0:n0}", Convert.ToInt64((expenseCost.FLOAT_ETC_AMT / 1000)));   //기타    
            expenseCostViewModel.FixTotalAmt = string.Format("{0:n0}", Convert.ToInt64(((expenseCost.MAN_AMT + expenseCost.SALES_AMT + expenseCost.STATIC_ETC_AMT) / 1000)));   //고정경비 합계
            expenseCostViewModel.MoveTotalAmt = string.Format("{0:n0}", Convert.ToInt64(((expenseCost.WELFARE_AMT + expenseCost.TAX_AMT + expenseCost.WASTE_AMT + expenseCost.FLOAT_ETC_AMT) / 1000)));   //유동경비 합계

            return expenseCostViewModel;
        }

        //주요매출 Model 생성
        public static IList<TaxSalesViewModel> MakeTaxSalseListViewModel(IList<SHUSER_SboMonthlyTaxSalesSelectReturnModel> taxSalesList, IList<SHUSER_SboMonthlySalesSelectReturnModel> slaesList)
        {
            IList<TaxSalesViewModel> taxSalesListViewModel = new List<TaxSalesViewModel>();
            foreach (var taxSales in taxSalesList)
            {
                TaxSalesViewModel taxSalesViewModel = new TaxSalesViewModel();
                taxSalesViewModel.CustName = taxSales.ACPT_TR_NM; //매입자 회사명
                taxSalesViewModel.WriteDate = taxSales.JNLYZ_DT.Substring(0, 4) + "-" + taxSales.JNLYZ_DT.Substring(4, 2) + "-" + taxSales.JNLYZ_DT.Substring(6, 2); ; //작성일자
                taxSalesViewModel.ItemName = taxSales.ITM_NM; //품목명
                taxSalesViewModel.TotalAmt = string.Format("{0:n0}", Convert.ToInt64((taxSales.SUM_AMT / 1000)));   //합계금액

                if (slaesList[0].SALES_AMT != 0)
                {
                    taxSalesViewModel.Share = string.Format("{0:n0}", Convert.ToInt64((taxSales.SUM_AMT / slaesList[0].SALES_AMT) * 100));
                }
                else
                {
                    taxSalesViewModel.Share = "0";
                }

                taxSalesListViewModel.Add(taxSalesViewModel);
            }
            return taxSalesListViewModel;
        }

        //주요지울 Model 생성
        public static IList<BankOutViewModel> MakeBnakOutListViewModel(IList<SHUSER_SboMonthlyBankOutSelectReturnModel> bankOutList)
        {
            IList<BankOutViewModel> bankOutListViewModel = new List<BankOutViewModel>();
            foreach (var bankOut in bankOutList)
            {
                BankOutViewModel bankOutViewModel = new BankOutViewModel();
                bankOutViewModel.BankName = bankOut.BANK_CD; //은행명
                bankOutViewModel.ItemName = bankOut.HISTCD_4; //적요
                bankOutViewModel.OutDate = bankOut.TRANDATE.Substring(0, 4) + "-" + bankOut.TRANDATE.Substring(4, 2) + "-" + bankOut.TRANDATE.Substring(6, 2); //출금일
                bankOutViewModel.TotalAmt = string.Format("{0:n0}", Convert.ToInt64((bankOut.HISTCD_O / 1000)));   //금액

                if (bankOut.TOTAL_SUM.Value != 0)
                {
                    bankOutViewModel.Share = string.Format("{0:n0}", Convert.ToInt64((bankOut.HISTCD_O / bankOut.TOTAL_SUM.Value) * 100));
                }
                else
                {
                    bankOutViewModel.Share = "0";
                }
                bankOutListViewModel.Add(bankOutViewModel);
            }
            return bankOutListViewModel;
        }

        public static QuarterModel CalcBeforQuarter(int year, int month)
        {
            QuarterModel qm = new QuarterModel();
            if (month >= 1 && month <= 3)
            {
                qm.Year = year - 1;
                qm.Quarter = 4;
            }
            else if (month >= 4 && month <= 6)
            {
                qm.Year = year;
                qm.Quarter = 1;
            }
            else if (month >= 7 && month <= 9)
            {
                qm.Year = year;
                qm.Quarter = 2;
            }
            else
            {
                qm.Year = year;
                qm.Quarter = 3;
            }


            return qm;
        }


        public static CompnayStatsViewModel MakeMonthCompnayStatsViewModel(ScCompMapping scCompMapping, SHUSER_SboFinancialTab1SalesSelectReturnModel monthSales)
        {
            CompnayStatsViewModel model = new CompnayStatsViewModel();
            model.CompNm = scCompMapping.ScCompInfo.CompNm;
            model.AvgSales = Math.Truncate(monthSales.TERM_SALE_AVR.Value / 1000).ToString();
            model.BeforeSales = Math.Truncate(monthSales.PRE_TO_SALE.Value / 1000).ToString();
            model.CntEmploy = Math.Truncate(monthSales.QT_EMP.Value).ToString();
            model.LastSales = Math.Truncate(monthSales.TO_SALE.Value).ToString();
            model.SumSales = Math.Truncate(monthSales.TERM_SALE.Value).ToString();

            return model;
        }

        public static CompnayStatsViewModel MakeQuarterCompnayStatsViewModel(ScCompMapping scCompMapping, SHUSER_SboFinancialTab2SalesSelectReturnModel quarterSales)
        {
            CompnayStatsViewModel model = new CompnayStatsViewModel();
            model.CompNm = scCompMapping.ScCompInfo.CompNm;
            model.AvgSales = Math.Truncate(quarterSales.QT_AVR.Value / 1000).ToString();
            model.BeforeSales = Math.Truncate(quarterSales.PRE_TO_SALE.Value / 1000).ToString();
            model.CntEmploy = Math.Truncate(quarterSales.QT_EMP.Value).ToString();
            model.LastSales = Math.Truncate(quarterSales.TO_SALE.Value).ToString();
            model.SumSales = Math.Truncate(quarterSales.TERM_SALE.Value).ToString();

            return model;
        }

        public static CompnayStatsViewModel MakeYearCompnayStatsViewModel(ScCompMapping scCompMapping, SHUSER_SboFinancialTab3SalesSelectReturnModel yearSales)
        {
            CompnayStatsViewModel model = new CompnayStatsViewModel();
            model.CompNm = scCompMapping.ScCompInfo.CompNm;
            model.AvgSales = Math.Truncate(yearSales.TERM_SALE_AVR.Value / 1000).ToString();
            model.BeforeSales = Math.Truncate(yearSales.PRE_TO_SALE.Value / 1000).ToString();
            model.CntEmploy = Math.Truncate(yearSales.QT_EMP.Value).ToString();
            model.LastSales = Math.Truncate(yearSales.TO_SALE.Value).ToString();
            model.SumSales = Math.Truncate(yearSales.TERM_SALE.Value).ToString();

            return model;
        }

        public static object[] MakeProcedureParams(string bpNo, string corpCd, string bizCd, string year, string month)
        {
            SqlParameter compRegNo = new SqlParameter("MEMB_BUSNPERS_NO", bpNo);
            SqlParameter corpCode = new SqlParameter("CORP_CODE", corpCd);
            SqlParameter bizCode = new SqlParameter("BIZ_CD", bizCd);
            SqlParameter setYear = new SqlParameter("SET_YEAR", year);
            SqlParameter setMonth = new SqlParameter("SET_MONTH", month);

            object[] parameters = new object[] { compRegNo, corpCode, bizCode, setYear, setMonth };

            return parameters;
        }

        public static object[] MakeSalesMonthProcedureParams(string bpNo, string corpCd, string bizCd, string startYear, string startMonth, string endYear, string endMonth)
        {
            if (startMonth.Length == 1)
                startMonth = "0" + startMonth;

            if (endMonth.Length == 1)
                endMonth = "0" + endMonth;

            DateTime lastday = new DateTime(int.Parse(endYear), int.Parse(endMonth), DateTime.DaysInMonth(int.Parse(endYear), int.Parse(endMonth)));

            SqlParameter compRegNo = new SqlParameter("MEMB_BUSNPERS_NO", bpNo);
            SqlParameter corpCode = new SqlParameter("CORP_CODE", corpCd);
            SqlParameter bizCode = new SqlParameter("BIZ_CD", bizCd);
            SqlParameter startYM = new SqlParameter("FR_YM", startYear + startMonth);
            SqlParameter endYM = new SqlParameter("TO_YM", endYear + endMonth);
            SqlParameter baseDt = new SqlParameter("BASE_DT", lastday.ToString("yyyyMMdd"));

            object[] parameters = new object[] { compRegNo, corpCode, bizCode, startYM, endYM, baseDt };

            return parameters;
        }

        public static object[] MakeSalesQuarterProcedureParams(string bpNo, string corpCd, string bizCd, string startYear, string startQuarter, string endYear, string endQuarter)
        {
            int endMonth = 0;

            if (endQuarter == "1")
                endMonth = 3;
            else if (endQuarter == "2")
                endMonth = 6;
            else if (endQuarter == "3")
                endMonth = 9;
            else
                endMonth = 12;

            DateTime lastday = new DateTime(int.Parse(endYear), endMonth, DateTime.DaysInMonth(int.Parse(endYear), endMonth));

            SqlParameter compRegNo = new SqlParameter("MEMB_BUSNPERS_NO", bpNo);
            SqlParameter corpCode = new SqlParameter("CORP_CODE", corpCd);
            SqlParameter bizCode = new SqlParameter("BIZ_CD", bizCd);
            SqlParameter startY = new SqlParameter("FR_YEAR", startYear);
            SqlParameter startQ = new SqlParameter("FR_QT", startQuarter);
            SqlParameter endY = new SqlParameter("TO_YEAR", endYear);
            SqlParameter endQ = new SqlParameter("TO_QT", endQuarter);
            SqlParameter baseDt = new SqlParameter("BASE_DT", lastday.ToString("yyyyMMdd"));

            object[] parameters = new object[] { compRegNo, corpCode, bizCode, startY, startQ, endY, endQ, baseDt };

            return parameters;
        }

        public static object[] MakeSalesYearProcedureParams(string bpNo, string corpCd, string bizCd, string startYear, string endYear)
        {
            DateTime lastday = new DateTime(int.Parse(endYear), 12, DateTime.DaysInMonth(int.Parse(endYear), 12));

            SqlParameter compRegNo = new SqlParameter("MEMB_BUSNPERS_NO", bpNo);
            SqlParameter corpCode = new SqlParameter("CORP_CODE", corpCd);
            SqlParameter bizCode = new SqlParameter("BIZ_CD", bizCd);
            SqlParameter startYM = new SqlParameter("FR_YEAR", startYear);
            SqlParameter endYM = new SqlParameter("TO_YEAR", endYear);
            SqlParameter baseDt = new SqlParameter("BASE_DT", lastday.ToString("yyyyMMdd"));

            object[] parameters = new object[] { compRegNo, corpCode, bizCode, startYM, endYM, baseDt };

            return parameters;
        }

        public static RptMentorComment MakeRptMentorcomment(CommentViewModel viewModel, BasicSurveyReportViewModel paramModel)
        {
            RptMentorComment rptComment = new RptMentorComment();
            rptComment.DetailCd = viewModel.DetailCd;
            rptComment.Comment = viewModel.Comment == null ? "" : viewModel.Comment;
            rptComment.BasicYear = paramModel.BizWorkYear;
            rptComment.BizWorkSn = paramModel.BizWorkSn;
            rptComment.QuestionSn = paramModel.QuestionSn;

            return rptComment;
        }

        public static RptMentorComment MakeRptMentorcomment(CheckBoxViewModel viewModel, BasicSurveyReportViewModel paramModel)
        {
            RptMentorComment rptComment = new RptMentorComment();
            rptComment.DetailCd = viewModel.DetailCd;
            rptComment.Comment = viewModel.CheckVal.ToString();
            rptComment.BasicYear = paramModel.BizWorkYear;
            rptComment.BizWorkSn = paramModel.BizWorkSn;
            rptComment.QuestionSn = paramModel.QuestionSn;

            return rptComment;
        }

        public static CommentViewModel MakeCommentViewModel(BasicSurveyReportViewModel paramModel, string detailCode, RptMentorComment rptMentorComment = null)
        {
            CommentViewModel viewModel = new CommentViewModel();

            if (rptMentorComment == null)
            {
                viewModel.DetailCd = detailCode;
                viewModel.Comment = "";
            }
            else
            {
                viewModel.DetailCd = rptMentorComment.DetailCd;
                viewModel.Comment = rptMentorComment.Comment;
            }

            return viewModel;
        }

        public static CommentViewModel MakeCommentViewModel(string detailCode, RptMngComment rptMngComment = null)
        {
            CommentViewModel viewModel = new CommentViewModel();

            if (rptMngComment == null)
            {
                viewModel.DetailCd = detailCode;
                viewModel.Comment = "";
            }
            else
            {
                viewModel.DetailCd = rptMngComment.DetailCd;
                viewModel.Comment = rptMngComment.Comment;
            }

            return viewModel;
        }

        public static CheckBoxViewModel MakeCheckBoxViewModel(BasicSurveyReportViewModel paramModel, string detailCode, RptMentorComment rptMentorComment = null)
        {
            var viewModel = new CheckBoxViewModel();

            if (rptMentorComment == null)
            {
                viewModel.DetailCd = detailCode;
                viewModel.CheckVal = false;
            }
            else
            {
                viewModel.DetailCd = rptMentorComment.DetailCd;
                viewModel.CheckVal = bool.Parse(rptMentorComment.Comment);
            }

            return viewModel;
        }

        public static IList<CommentViewModel> MakeCommentViewModel(IEnumerable<RptMngCode> listRptMngCode, IList<RptMngComment> listRptMngComment)
        {
            var CommentList = new List<CommentViewModel>();

            foreach (var rptCheckList in listRptMngCode)
            {
                var rptMngComment = listRptMngComment.SingleOrDefault(rmc => rmc.DetailCd == rptCheckList.DetailCd);

                CommentList.Add(MakeCommentViewModel(rptCheckList.DetailCd, rptMngComment));
            }

            return CommentList;

        }

        public static IList<CommentViewModel> MakeRmkCommentViewModel(IEnumerable<RptMngCode> listRptMngCode, IList<RptMngComment> listRptMngComment)
        {
            var CommentList = new List<CommentViewModel>();

            foreach (var rptCheckList in listRptMngCode)
            {
                var rptMngComment = listRptMngComment.SingleOrDefault(rmc => rmc.DetailCd == rptCheckList.DetailCd);

                CommentList.Add(MakeCommentViewModel(rptCheckList.DetailCd, rptMngComment));
            }

            return CommentList;

        }


        public static IList<CommentViewModel> MakeCommentViewModel(IEnumerable<RptCheckList> listRptCheckList, IList<RptMentorComment> listRptMentorComment)
        {
            var CommentList = new List<CommentViewModel>();

            foreach (var rptCheckList in listRptCheckList)
            {
                var rptMentorComment = listRptMentorComment.SingleOrDefault(rmc => rmc.DetailCd == rptCheckList.DetailCd);
                CommentList.Add(MakeCommentViewModel(null, rptCheckList.DetailCd, rptMentorComment));
            }
            
            return CommentList;

        }

        public static IList<CheckBoxViewModel> MakeCheckBoxViewModel(IEnumerable<RptCheckList> listRptCheckList, IList<RptMentorComment> listRptMentorComment)
        {
            var CheckBoxList = new List<CheckBoxViewModel>();

            foreach (var rptCheckList in listRptCheckList)
            {
                var rptMentorComment = listRptMentorComment.SingleOrDefault(rmc => rmc.DetailCd == rptCheckList.DetailCd);

                CheckBoxList.Add(MakeCheckBoxViewModel(null, rptCheckList.DetailCd, rptMentorComment));
            }

            return CheckBoxList;

        }

        public static int CalcCheckCount(IList<QuesResult1> checkList)
        {
            int trueCount = 0;
            foreach (var check in checkList)
            {
                if (check.AnsVal == true)
                {
                    trueCount++;
                }
            }

            return trueCount;
        }

        public static string GetCodeTypeA(int trueCnt)
        {
            string code = "";
            switch (trueCnt)
            {
                case 0:
                case 1:
                    code = "E";
                    break;
                case 2:
                    code = "D";
                    break;
                case 3:
                    code = "C";
                    break;
                case 4:
                    code = "B";
                    break;
                default:
                    code = "A";
                    break;

            }
            return code;
        }

        public static string GetCodeTypeB(int Cnt)
        {
            string code = "";
            switch (Cnt)
            {
                case 0:
                    code = "E";
                    break;
                case 1:
                    code = "C";
                    break;
                default:
                    code = "A";
                    break;

            }
            return code;
        }

        public static string GetCodeTypeC(int trueCnt)
        {
            string code = "";
            switch (trueCnt)
            {
                case 0:
                    code = "E";
                    break;
                case 1:
                    code = "D";
                    break;
                case 2:
                    code = "C";
                    break;
                case 3:
                    code = "B";
                    break;
                default:
                    code = "A";
                    break;

            }
            return code;
        }


        public static string GetCodeTypeD(double per)
        {
            string code = "";
            if (per < 5)
                code = "A";
            else if (per >= 5 && per < 10)
                code = "B";
            else if (per >= 10 && per < 15)
                code = "C";
            else if (per >= 15 && per < 20)
                code = "D";
            else
                code = "E";
            return code;
        }


        public static string GetCodeTypeE(double per)
        {
            string code = "";
            if (per >= 5)
                code = "A";
            else if (per < 5 && per >= 4)
                code = "B";
            else if (per < 4 && per >= 3)
                code = "C";
            else if (per < 3 && per >= 2)
                code = "D";
            else
                code = "E";
            return code;
        }

        public static string GetCodeTypeF(double per)
        {
            string code = "";
            if (per >= 4)
                code = "A";
            else if (per < 4 && per >= 3)
                code = "B";
            else if (per < 3 && per >= 2)
                code = "C";
            else if (per < 2 && per >= 1)
                code = "D";
            else
                code = "E";
            return code;
        }

        public static string GetCodeTypeG(int trueCnt)
        {
            string code = "";
            switch (trueCnt)
            {
                case 0:
                case 1:
                case 2:
                    code = "E";
                    break;
                case 3:
                    code = "D";
                    break;
                case 4:
                    code = "C";
                    break;
                case 5:
                    code = "B";
                    break;
                default:
                    code = "A";
                    break;

            }
            return code;
        }

        public static string GetCodeTypeH(double per)
        {
            string code = "";
            if (per >= 25)
                code = "A";
            else if (per < 25 && per >= 20)
                code = "B";
            else if (per < 20 && per >= 15)
                code = "C";
            else if (per < 15 && per >= 10)
                code = "D";
            else
                code = "E";
            return code;
        }

        public static string GetCodeTypeI(double per)
        {
            string code = "";
            if (per >= 7.5)
                code = "A";
            else if (per < 7.5 && per >= 5)
                code = "B";
            else if (per < 5 && per >= 2.5)
                code = "C";
            else if (per < 2.5 && per >= 0.1)
                code = "D";
            else
                code = "E";
            return code;
        }

        public static string GetCodeTypeM(double per)
        {
            string code = "";
            if (per >= 3)
                code = "A";
            else if (per < 3 && per >= 2.5)
                code = "B";
            else if (per < 2.5 && per >= 2)
                code = "C";
            else if (per < 2 && per >= 1.5)
                code = "D";
            else
                code = "E";
            return code;
        }

        public static double CalcPoint(string type, double standardPoint)
        {
            double point = 0;
            switch (type)
            {
                case "A":
                    point = standardPoint * 1;
                    break;
                case "B":
                    point = standardPoint * 0.75;
                    break;
                case "C":
                    point = standardPoint * 0.5;
                    break;
                case "D":
                    point = standardPoint * 0.25;
                    break;
                case "E":
                    point = standardPoint * 0.0;
                    break;
            }
            return point;
        }


        public static string GetArrowTypeA(double point)
        {
            string code = "";
            if (point >= 0 && point <= 50)
                code = "C";
            else if (point > 50 && point <= 75)
                code = "B";
            else
                code = "A";
            return code;
        }

        public static string GetArrowTypeB(double point)
        {
            string code = "";
            if (point >= 0 && point <= 9.5)
                code = "C";
            else if (point > 9.5 && point <= 14.25)
                code = "B";
            else
                code = "A";
            return code;
        }

        public static string GetArrowTypeC(double point)
        {
            string code = "";
            if (point >= 0 && point <= 19.5)
                code = "C";
            else if (point > 19.5 && point <= 29.25)    // 0411 - phm , 29.25로 변경
                code = "B";
            else
                code = "A";
            return code;
        }

        public static string GetArrowTypeD(double point)
        {
            string code = "";
            if (point >= 0 && point <= 5)
                code = "C";
            else if (point > 5 && point <= 7.5)
                code = "B";
            else
                code = "A";
            return code;
        }

        public static string GetArrowTypeE(int type)
        {
            string code = "";
            if (type == 1)
                code = "C";
            else if (type == 2)
                code = "B";
            else
                code = "A";
            return code;
        }

        public static double CalcFinancialPoint(SHUSER_SboFinancialIndexT sboFinancialIndexT, ScCav obj)
        {
            //각 항목의 계산값이 0보다 작을경우는 0점으로 처리한다.


            //매출영업이익률(영업이익 ÷ 매출액)×100
            //if(sboFinancialIndexT.CurrentSale.Value == 0) ? 
            double a = (sboFinancialIndexT.CurrentSale.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.OperatingEarning.Value / sboFinancialIndexT.CurrentSale.Value) * 100);
            double aPoint = 0.0;

            if (sboFinancialIndexT.OperatingEarning.Value == 0  || sboFinancialIndexT.CurrentSale.Value == 0 )
            {
                aPoint = 0;
            }
            else
            {
                // 소수 3.4000000004 식으로 왜??
                //double ex = Convert.ToDouble(obj.CavOp);
                //double gw = getWeight(a, ex);
                //aPoint = gw * 17D;
                aPoint = getWeight(a, Convert.ToDouble(obj.CavOp)) * 17; 
            }
            //double aPoint = (a / (5.2 + a)) * 17;

            //자기자본순이익률(당기순이익 ÷ 자본총계)×100
            double b = (sboFinancialIndexT.TotalCapital.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentEarning.Value / sboFinancialIndexT.TotalCapital.Value) * 100);
            double bPoint = 0.0;

            if(sboFinancialIndexT.CurrentEarning.Value == 0 || sboFinancialIndexT.TotalCapital.Value == 0)
            {
                bPoint = 0;
            }
            else
            {
                bPoint = getWeight(b, Convert.ToDouble(obj.CavRe)) * 6;
            }
            //double bPoint = (b / (5.19 + b)) * 6;

            //매출증가율((당기매출액 - 전기매출액) ÷ 전기매출액)×100
            double c = (sboFinancialIndexT.PrevSale.Value == 0) ? 0 : Convert.ToDouble(((sboFinancialIndexT.CurrentSale.Value - sboFinancialIndexT.PrevSale.Value) / sboFinancialIndexT.PrevSale.Value) * 100);
            //double cPoint = ((c / (4.93 + c)) * 9);
            double cPoint = 0.0;

            //전기매출액이 0 이고 당기매출액이 0 일때 =  0점
            if (sboFinancialIndexT.PrevSale.Value == 0)
            {
                if (sboFinancialIndexT.CurrentSale.Value == 0)
                {
                    cPoint = 0;
                }
                else if (sboFinancialIndexT.CurrentSale > 0)
                {
                    cPoint = 9;
                }
            }
            else
            {
                cPoint = getWeight(c, Convert.ToDouble(obj.CavSg)) * 9;
            }

            //순이익증가율((당기순이익 - 전기순이익) ÷ 전기순이익)×100
            double d = (sboFinancialIndexT.PrevEarning.Value == 0) ? 0 : Convert.ToDouble(((sboFinancialIndexT.CurrentEarning.Value - sboFinancialIndexT.PrevEarning.Value) / sboFinancialIndexT.PrevEarning.Value) * 100);
            //double dPoint = (d / (19.96 + d)) * 14;
            double dPoint = 0.0;

            //당기손익이 0이하일때 = 0점
            if (sboFinancialIndexT.CurrentEarning.Value <= 0)
            {
                dPoint = 0;
            }
            //당기손익이 이익(양수)이고 전기손익이 0이하일때 = 14점
            else if (sboFinancialIndexT.CurrentEarning.Value > 0 && sboFinancialIndexT.PrevEarning.Value <= 0)
            {
                dPoint = 14;
            }
            else
            {
                dPoint = getWeight(d, (Convert.ToDouble(obj.CavNg))) * 14;
            }
            
            //당좌비율((유동자산 - 재고자산) ÷ 유동부채)×100
            double e = (sboFinancialIndexT.CurrentLiability.Value == 0) ? 0 : Convert.ToDouble(((sboFinancialIndexT.CurrentAsset.Value - sboFinancialIndexT.InventoryAsset.Value) / sboFinancialIndexT.CurrentLiability.Value) * 100);
            double ePoint = 0.0;
            // double ePoint = (e / (102.09 + e)) * 4;

            // 유동부채가 0일 경우 4점
            if ((sboFinancialIndexT.CurrentAsset.Value - sboFinancialIndexT.InventoryAsset.Value) == 0 && sboFinancialIndexT.CurrentLiability.Value == 0)
            {
                ePoint = 0;
            }
            else if ((sboFinancialIndexT.CurrentAsset.Value - sboFinancialIndexT.InventoryAsset.Value) == 0)
            {
                ePoint = 0;
            }
            else
            { 
                if(sboFinancialIndexT.CurrentLiability.Value == 0)
                {
                    ePoint = 4;
                }
                else
                {
                    ePoint = getWeight(e, Convert.ToDouble(obj.CavQr)) * 4;
                }
            }

            //유동비율(유동자산 ÷ 유동부채)×100 
            double f = (sboFinancialIndexT.CurrentLiability.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentAsset.Value / sboFinancialIndexT.CurrentLiability.Value) * 100);
            double fPoint = 0.0;

            // 유동부채가 0일 경우 13점
            if (sboFinancialIndexT.CurrentAsset.Value == 0 && sboFinancialIndexT.CurrentLiability.Value == 0)
            {
                fPoint = 0;
            }
            else if (sboFinancialIndexT.CurrentAsset.Value == 0)
            {
                fPoint = 0;
            }
            else
            {
                if (sboFinancialIndexT.CurrentLiability.Value == 0)
                {
                    fPoint = 13;
                }
                else
                {
                    fPoint = getWeight(f, (Convert.ToDouble(obj.CavCr)))*13;
                }
                //double fPoint = (f / (136.27 + f)) * 13;
            }

            //부채비율(부채 ÷ 자본총계)×100
            double g = (sboFinancialIndexT.TotalCapital.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.TotalLiability.Value / sboFinancialIndexT.TotalCapital.Value) * 100);
            double gPoint = 0.0;

            if (sboFinancialIndexT.TotalLiability.Value == 0 && sboFinancialIndexT.TotalCapital.Value == 0)
            {
                gPoint = 0;
            }
            else if (sboFinancialIndexT.TotalLiability.Value == 0)
            {
                gPoint = 9;
            }
            else
            {
                gPoint = getWeight(g, Convert.ToDouble(obj.CavDebt)) * 9;
            }

            //이자보상비율(영업이익 ÷ 이자비용)×100
            double h = (sboFinancialIndexT.InterstCost.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.OperatingEarning.Value / sboFinancialIndexT.InterstCost.Value) * 100);
            double hPoint = 0;

            if (sboFinancialIndexT.OperatingEarning.Value == 0 && sboFinancialIndexT.InterstCost.Value == 0)
            {
                hPoint = 0;
            }
            else if (sboFinancialIndexT.OperatingEarning.Value == 0)
            {
                hPoint = 0;
            }
            else
            {
                // 이자비용이 0일 경우 7
                if (sboFinancialIndexT.InterstCost.Value == 0)
                {
                    hPoint = 7;
                }
                else
                {
                    hPoint = getWeight(h, Convert.ToDouble(obj.CavIcr)) * 7;
                }
                //double hPoint = (h / (333.63 + h)) * 7;
            }

            //총자산회전율(매출액 ÷ 총자산)×100
            double i = (sboFinancialIndexT.TotalAsset.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentSale.Value / sboFinancialIndexT.TotalAsset.Value) * 100);
            double iPoint = 0.0;

            if (sboFinancialIndexT.CurrentSale.Value == 0 && sboFinancialIndexT.TotalAsset.Value == 0)
            {
                iPoint = 0;
            }
            else if (sboFinancialIndexT.CurrentSale.Value == 0)
            {
                iPoint = 0;
            }
            else
            {
                // 총 자산이 0일 경우
                if (sboFinancialIndexT.TotalAsset.Value == 0)
                {
                    iPoint = 3;
                }
                else
                {
                    iPoint = getWeight(i, Convert.ToDouble(obj.CavTat)) * 3;
                }
                //double iPoint = (i / (114.75 + i)) * 3;
            }

            //매출채권회전율(매출액 ÷ 매출채권(=외상매출금,미수금,받을어음))×100
            double j = (sboFinancialIndexT.SalesCredit.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentSale.Value / sboFinancialIndexT.SalesCredit.Value) * 100);
            double jPoint = 0;

            if (sboFinancialIndexT.CurrentSale.Value == 0 && sboFinancialIndexT.SalesCredit.Value == 0)
            {
                jPoint = 0;
            }
            else if (sboFinancialIndexT.CurrentSale.Value == 0)
            {
                jPoint = 0;
            }
            else
            {
                // 매출채권이 0인 경우
                if (sboFinancialIndexT.SalesCredit.Value == 0)
                {
                    jPoint = 3;
                }
                else
                {
                    jPoint = getWeight(j, Convert.ToDouble(obj.CavTrt)) * 3;
                }
                //double jPoint = (j / (569.36 + j)) * 3;
            }

            //재고자산회전율(매출액 ÷ 재고자산)×100
            double k = (sboFinancialIndexT.InventoryAsset.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentSale.Value / sboFinancialIndexT.InventoryAsset.Value) * 100);
            double kPoint = 0;

            if (sboFinancialIndexT.CurrentSale.Value == 0 && sboFinancialIndexT.InventoryAsset.Value == 0)
            {
                kPoint = 0;
            }
            else if (sboFinancialIndexT.CurrentSale.Value == 0)
            {
                kPoint = 0;
            }
            else
            {
                //재고자산이 0일때 = 4점
                if (sboFinancialIndexT.InventoryAsset.Value == 0)
                {
                    kPoint = 4;
                }
                else
                {
                    kPoint = getWeight(k, Convert.ToDouble(obj.CavIt)) * 4;
                }
                //double kPoint = (k / (915.48 + k)) * 4;
            }

            // 부가가치율(부가가치 ÷ 매출액)×100
            double l = (sboFinancialIndexT.CurrentSale.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.ValueAdded.Value / sboFinancialIndexT.CurrentSale.Value) * 100);
            double lPoint = 0;

            if (sboFinancialIndexT.ValueAdded.Value == 0 && sboFinancialIndexT.CurrentSale.Value == 0)
            {
                lPoint = 0;
            }
            else if (sboFinancialIndexT.ValueAdded.Value == 0)
            {
                lPoint = 0;
            }
            else
            {
                // 당기매출액이 0이면
                if (sboFinancialIndexT.CurrentSale.Value == 0)
                {
                    lPoint = 4;
                }
                else
                {
                    lPoint = getWeight(l, (Convert.ToDouble(obj.CavVr))) * 4;
                }
                //double lPoint = (l / (24.02 + l)) * 4;
            }

            // 노동생산성 = 부가가치 ÷ 종업원수
            double m = (sboFinancialIndexT.QtEmp.Value == 0) ? 0 : Convert.ToDouble(sboFinancialIndexT.ValueAdded.Value / sboFinancialIndexT.QtEmp.Value);
            double mPoint = 0;

            if (sboFinancialIndexT.ValueAdded.Value == 0 && sboFinancialIndexT.QtEmp.Value == 0)
            {
                mPoint = 0;
            }
            else if (sboFinancialIndexT.ValueAdded.Value == 0)
            {
                mPoint = 0;
            }
            else
            {
                // 종업원 수가 0이면
                if (sboFinancialIndexT.QtEmp.Value == 0)
                {
                    mPoint = 4;
                }
                else
                {
                    mPoint = getWeight(Math.Truncate(m/1000), Convert.ToDouble(obj.CavLp)) * 4;
                }
            }

            //double mPoint = (m / (16163671 + m)) * 4;

            //자본생산성((부가가치 ÷ 자본총계)×100
            double n = (sboFinancialIndexT.TotalCapital.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.ValueAdded.Value / sboFinancialIndexT.TotalCapital.Value) * 100);
            double nPoint = 0;

            if (sboFinancialIndexT.ValueAdded.Value == 0 && sboFinancialIndexT.TotalCapital.Value == 0)
            {
                nPoint = 0;
            }
            else if (sboFinancialIndexT.ValueAdded.Value == 0)
            {
                nPoint = 0;
            }
            else
            {
                // 자본총계가 0이면
                if (sboFinancialIndexT.TotalCapital.Value == 0)
                {
                    nPoint = 3;
                }
                else
                {
                    nPoint = getWeight(n, Convert.ToDouble(obj.CavCp)) * 3;
                }
                //double nPoint = (n / (137.01 + n)) * 3;
            }

            //재무점수로 환산
            double point = aPoint + bPoint + cPoint + dPoint + ePoint + fPoint + gPoint + hPoint + iPoint + jPoint + kPoint + lPoint + mPoint + nPoint;
            return point;
        }

        public static double CalcFinancialPoint(ScFinancialIndexT sboFinancialIndexT, ScCav obj)
        {

            //매출영업이익률(영업이익 ÷ 매출액)×100
            //if(sboFinancialIndexT.CurrentSale.Value == 0) ? 
            double a = (sboFinancialIndexT.CurrentSale.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.OperatingEarning.Value / sboFinancialIndexT.CurrentSale.Value) * 100);
            double aPoint = 0.0;

            if (sboFinancialIndexT.OperatingEarning.Value == 0 || sboFinancialIndexT.CurrentSale.Value == 0)
            {
                aPoint = 0;
            }
            else
            {
                // 소수 3.4000000004 식으로 왜??
                //double ex = Convert.ToDouble(obj.CavOp);
                //double gw = getWeight(a, ex);
                //aPoint = gw * 17D;
                aPoint = getWeight(a, Convert.ToDouble(obj.CavOp)) * 17;
            }
            //double aPoint = (a / (5.2 + a)) * 17;

            //자기자본순이익률(당기순이익 ÷ 자본총계)×100
            double b = (sboFinancialIndexT.TotalCapital.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentEarning.Value / sboFinancialIndexT.TotalCapital.Value) * 100);
            double bPoint = 0.0;

            if (sboFinancialIndexT.CurrentEarning.Value == 0 || sboFinancialIndexT.TotalCapital.Value == 0)
            {
                bPoint = 0;
            }
            else
            {
                bPoint = getWeight(b, Convert.ToDouble(obj.CavRe)) * 6;
            }
            //double bPoint = (b / (5.19 + b)) * 6;

            //매출증가율((당기매출액 - 전기매출액) ÷ 전기매출액)×100
            double c = (sboFinancialIndexT.PrevSale.Value == 0) ? 0 : Convert.ToDouble(((sboFinancialIndexT.CurrentSale.Value - sboFinancialIndexT.PrevSale.Value) / sboFinancialIndexT.PrevSale.Value) * 100);
            //double cPoint = ((c / (4.93 + c)) * 9);
            double cPoint = 0.0;

            //전기매출액이 0 이고 당기매출액이 0 일때 =  0점
            if (sboFinancialIndexT.PrevSale.Value == 0)
            {
                if (sboFinancialIndexT.CurrentSale.Value == 0)
                {
                    cPoint = 0;
                }
                else if (sboFinancialIndexT.CurrentSale > 0)
                {
                    cPoint = 9;
                }
            }
            else
            {
                cPoint = getWeight(c, Convert.ToDouble(obj.CavSg)) * 9;
            }

            //순이익증가율((당기순이익 - 전기순이익) ÷ 전기순이익)×100
            double d = (sboFinancialIndexT.PrevEarning.Value == 0) ? 0 : Convert.ToDouble(((sboFinancialIndexT.CurrentEarning.Value - sboFinancialIndexT.PrevEarning.Value) / sboFinancialIndexT.PrevEarning.Value) * 100);
            //double dPoint = (d / (19.96 + d)) * 14;
            double dPoint = 0.0;

            //당기손익이 0이하일때 = 0점
            if (sboFinancialIndexT.CurrentEarning.Value <= 0)
            {
                dPoint = 0;
            }
            //당기손익이 이익(양수)이고 전기손익이 0이하일때 = 14점
            else if (sboFinancialIndexT.CurrentEarning.Value > 0 && sboFinancialIndexT.PrevEarning.Value <= 0)
            {
                dPoint = 14;
            }
            else
            {
                dPoint = getWeight(d, (Convert.ToDouble(obj.CavNg))) * 14;
            }

            //당좌비율((유동자산 - 재고자산) ÷ 유동부채)×100
            double e = (sboFinancialIndexT.CurrentLiability.Value == 0) ? 0 : Convert.ToDouble(((sboFinancialIndexT.CurrentAsset.Value - sboFinancialIndexT.InventoryAsset.Value) / sboFinancialIndexT.CurrentLiability.Value) * 100);
            double ePoint = 0.0;
            // double ePoint = (e / (102.09 + e)) * 4;

            // 유동부채가 0일 경우 4점
            if ((sboFinancialIndexT.CurrentAsset.Value - sboFinancialIndexT.InventoryAsset.Value) == 0 && sboFinancialIndexT.CurrentLiability.Value == 0)
            {
                ePoint = 0;
            }
            else if ((sboFinancialIndexT.CurrentAsset.Value - sboFinancialIndexT.InventoryAsset.Value) == 0)
            {
                ePoint = 0;
            }
            else
            {
                if (sboFinancialIndexT.CurrentLiability.Value == 0)
                {
                    ePoint = 4;
                }
                else
                {
                    ePoint = getWeight(e, Convert.ToDouble(obj.CavQr)) * 4;
                }
            }

            //유동비율(유동자산 ÷ 유동부채)×100 
            double f = (sboFinancialIndexT.CurrentLiability.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentAsset.Value / sboFinancialIndexT.CurrentLiability.Value) * 100);
            double fPoint = 0.0;

            // 유동부채가 0일 경우 13점
            if (sboFinancialIndexT.CurrentAsset.Value == 0 && sboFinancialIndexT.CurrentLiability.Value == 0)
            {
                fPoint = 0;
            }
            else if (sboFinancialIndexT.CurrentAsset.Value == 0)
            {
                fPoint = 0;
            }
            else
            {
                if (sboFinancialIndexT.CurrentLiability.Value == 0)
                {
                    fPoint = 13;
                }
                else
                {
                    fPoint = getWeight(f, (Convert.ToDouble(obj.CavCr))) * 13;
                }
                //double fPoint = (f / (136.27 + f)) * 13;
            }

            //부채비율(부채 ÷ 자본총계)×100
            double g = (sboFinancialIndexT.TotalCapital.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.TotalLiability.Value / sboFinancialIndexT.TotalCapital.Value) * 100);
            double gPoint = 0.0;

            if (sboFinancialIndexT.TotalLiability.Value == 0 && sboFinancialIndexT.TotalCapital.Value == 0)
            {
                gPoint = 0;
            }
            else if (sboFinancialIndexT.TotalLiability.Value == 0)
            {
                gPoint = 9;
            }
            else
            {
                gPoint = getWeight(g, Convert.ToDouble(obj.CavDebt)) * 9;
            }

            //이자보상비율(영업이익 ÷ 이자비용)×100
            double h = (sboFinancialIndexT.InterstCost.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.OperatingEarning.Value / sboFinancialIndexT.InterstCost.Value) * 100);
            double hPoint = 0;

            if (sboFinancialIndexT.OperatingEarning.Value == 0 && sboFinancialIndexT.InterstCost.Value == 0)
            {
                hPoint = 0;
            }
            else if (sboFinancialIndexT.OperatingEarning.Value == 0)
            {
                hPoint = 0;
            }
            else
            {
                // 이자비용이 0일 경우 7
                if (sboFinancialIndexT.InterstCost.Value == 0)
                {
                    hPoint = 7;
                }
                else
                {
                    hPoint = getWeight(h, Convert.ToDouble(obj.CavIcr)) * 7;
                }
                //double hPoint = (h / (333.63 + h)) * 7;
            }

            //총자산회전율(매출액 ÷ 총자산)×100
            double i = (sboFinancialIndexT.TotalAsset.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentSale.Value / sboFinancialIndexT.TotalAsset.Value) * 100);
            double iPoint = 0.0;

            if (sboFinancialIndexT.CurrentSale.Value == 0 && sboFinancialIndexT.TotalAsset.Value == 0)
            {
                iPoint = 0;
            }
            else if (sboFinancialIndexT.CurrentSale.Value == 0)
            {
                iPoint = 0;
            }
            else
            {
                // 총 자산이 0일 경우
                if (sboFinancialIndexT.TotalAsset.Value == 0)
                {
                    iPoint = 3;
                }
                else
                {
                    iPoint = getWeight(i, Convert.ToDouble(obj.CavTat)) * 3;
                }
                //double iPoint = (i / (114.75 + i)) * 3;
            }

            //매출채권회전율(매출액 ÷ 매출채권(=외상매출금,미수금,받을어음))×100
            double j = (sboFinancialIndexT.SalesCredit.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentSale.Value / sboFinancialIndexT.SalesCredit.Value) * 100);
            double jPoint = 0;

            if (sboFinancialIndexT.CurrentSale.Value == 0 && sboFinancialIndexT.SalesCredit.Value == 0)
            {
                jPoint = 0;
            }
            else if (sboFinancialIndexT.CurrentSale.Value == 0)
            {
                jPoint = 0;
            }
            else
            {
                // 매출채권이 0인 경우
                if (sboFinancialIndexT.SalesCredit.Value == 0)
                {
                    jPoint = 3;
                }
                else
                {
                    jPoint = getWeight(j, Convert.ToDouble(obj.CavTrt)) * 3;
                }
                //double jPoint = (j / (569.36 + j)) * 3;
            }

            //재고자산회전율(매출액 ÷ 재고자산)×100
            double k = (sboFinancialIndexT.InventoryAsset.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.CurrentSale.Value / sboFinancialIndexT.InventoryAsset.Value) * 100);
            double kPoint = 0;

            if (sboFinancialIndexT.CurrentSale.Value == 0 && sboFinancialIndexT.InventoryAsset.Value == 0)
            {
                kPoint = 0;
            }
            else if (sboFinancialIndexT.CurrentSale.Value == 0)
            {
                kPoint = 0;
            }
            else
            {
                //재고자산이 0일때 = 4점
                if (sboFinancialIndexT.InventoryAsset.Value == 0)
                {
                    kPoint = 4;
                }
                else
                {
                    kPoint = getWeight(k, Convert.ToDouble(obj.CavIt)) * 4;
                }
                //double kPoint = (k / (915.48 + k)) * 4;
            }

            // 부가가치율(부가가치 ÷ 매출액)×100
            double l = (sboFinancialIndexT.CurrentSale.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.ValueAdded.Value / sboFinancialIndexT.CurrentSale.Value) * 100);
            double lPoint = 0;

            if (sboFinancialIndexT.ValueAdded.Value == 0 && sboFinancialIndexT.CurrentSale.Value == 0)
            {
                lPoint = 0;
            }
            else if (sboFinancialIndexT.ValueAdded.Value == 0)
            {
                lPoint = 0;
            }
            else
            {
                // 당기매출액이 0이면
                if (sboFinancialIndexT.CurrentSale.Value == 0)
                {
                    lPoint = 4;
                }
                else
                {
                    lPoint = getWeight(l, (Convert.ToDouble(obj.CavVr))) * 4;
                }
                //double lPoint = (l / (24.02 + l)) * 4;
            }

            // 노동생산성 = 부가가치 ÷ 종업원수
            double m = (sboFinancialIndexT.QtEmp.Value == 0) ? 0 : Convert.ToDouble(sboFinancialIndexT.ValueAdded.Value / sboFinancialIndexT.QtEmp.Value);
            double mPoint = 0;

            if (sboFinancialIndexT.ValueAdded.Value == 0 && sboFinancialIndexT.QtEmp.Value == 0)
            {
                mPoint = 0;
            }
            else if (sboFinancialIndexT.ValueAdded.Value == 0)
            {
                mPoint = 0;
            }
            else
            {
                // 종업원 수가 0이면
                if (sboFinancialIndexT.QtEmp.Value == 0)
                {
                    mPoint = 4;
                }
                else
                {
                    mPoint = getWeight(Math.Truncate(m / 1000), Convert.ToDouble(obj.CavLp)) * 4;
                }
            }

            //double mPoint = (m / (16163671 + m)) * 4;

            //자본생산성((부가가치 ÷ 자본총계)×100
            double n = (sboFinancialIndexT.TotalCapital.Value == 0) ? 0 : Convert.ToDouble((sboFinancialIndexT.ValueAdded.Value / sboFinancialIndexT.TotalCapital.Value) * 100);
            double nPoint = 0;

            if (sboFinancialIndexT.ValueAdded.Value == 0 && sboFinancialIndexT.TotalCapital.Value == 0)
            {
                nPoint = 0;
            }
            else if (sboFinancialIndexT.ValueAdded.Value == 0)
            {
                nPoint = 0;
            }
            else
            {
                // 자본총계가 0이면
                if (sboFinancialIndexT.TotalCapital.Value == 0)
                {
                    nPoint = 3;
                }
                else
                {
                    nPoint = getWeight(n, Convert.ToDouble(obj.CavCp)) * 3;
                }
                //double nPoint = (n / (137.01 + n)) * 3;
            }

            //재무점수로 환산
            double point = aPoint + bPoint + cPoint + dPoint + ePoint + fPoint + gPoint + hPoint + iPoint + jPoint + kPoint + lPoint + mPoint + nPoint;
            return point;
        }

        //추가 테스트 부분
        public static SelectList cavSelectList(IList<ScCav> scCavList)
        {
            var cavList = new List<SelectListItem>();
            cavList.Add(new SelectListItem { Value = "0", Text = "년도", Selected = true });

            if (scCavList != null)
            {
                foreach (var item in scCavList)
                {
                    cavList.Add(new SelectListItem { Value = item.CavDt.ToString() });
                }
            }

            SelectList list = new SelectList(cavList, "Value", "Text");
            return list;
        }
        public static SelectList makSelectList(IList<ScMak> scMakList)
        {
            var makList = new List<SelectListItem>();
            makList.Add(new SelectListItem { Value = "0", Text = "년도", Selected = true });

            if (scMakList != null)
            {
                foreach (var item in scMakList)
                {
                    makList.Add(new SelectListItem { Value = item.MakDt.ToString() });
                }
            }

            SelectList list = new SelectList(makList, "Value", "Text");
            return list;
        }

        // 가중치 부분의 문제 확인 필요
        public static double getWeight(double value, double avg)
        {
            // ** 가중치의 문제는 평균값이 마이너스 일 때, 
            // ** 결국 계산되어 산출된 값의 앞에 -를 붙여 +를 만드는 것으로 생각해 볼 수 있다.
            double per = 0.0;       // 퍼센트 초기화
            double weight = 0.0;    // 가중치 초기화

            per = (value / avg) - 1;  // 퍼센트 구하기

            if (avg < 0 || (avg < 0 && value < 0))
            {
                per = per * -1;
            }
            
            if (per > 0.2)
            {
                weight = 1;
            }
            else if (per <= 0.2 && per > 0.1)
            {
                weight = 0.8;
            }
            else if (per <= 0.1 && per > -0.1)
            {
                weight = 0.6;
            }
            else if (per <= -0.1 && per >= -0.2)
            {
                weight = 0.4;
            }
            else
            {
                weight = 0.2;
            }
            return weight;
        }
        //navi 부분
        //public static RptPageListView SelectCommentViewModel(int questionSn, int bizworkSn , int? basicyear, string detailcd, string comment)
        //{
        //    var CommentList = new RptPageListView();

        //    CommentList.QuestionSn = questionSn;
        //    CommentList.BizWorkSn = bizworkSn;
        //    CommentList.BasicYear = basicyear;
        //    CommentList.DetailCd = detailcd;
        //    CommentList.Comment = comment;

        //    if(CommentList.Comment == null)
        //    {
        //        CommentList.NullCheck = "N";
        //    }
        //    else
        //    {
        //        CommentList.NullCheck = "Y";
        //    }
        //    return CommentList;

        //}

        // * 0411 경영자의 능력 - phm
        public static string getLeadershipScore(int officer)
        {
            if (officer >= 1)
            {
                return "A";
            }
            else
            {
                return "C";
            }
        }


    }
}