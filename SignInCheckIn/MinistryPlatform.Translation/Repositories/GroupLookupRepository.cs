using System;
using Crossroads.Utilities.Services.Interfaces;
using MinistryPlatform.Translation.Repositories.Interfaces;

namespace MinistryPlatform.Translation.Repositories
{
    public class GroupLookupRepository : IGroupLookupRepository
    {
        private readonly IApplicationConfiguration _applicationConfiguration;

        public GroupLookupRepository(IApplicationConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
        }

        public int GetGradeAttributeId(int gradeGroupId)
        {
            if (gradeGroupId != null)
            {
                switch (gradeGroupId)
                {
                    case 173938:
                        return 9033;
                    case 173937:
                        return 9034;
                    case 173936:
                        return 9035;
                    case 173935:
                        return 9036;
                    case 173934:
                        return 9037;
                    default:
                        // if no match, assume kindergarten?
                        return 9032;
                }
            }
            return 0;
        }

        public int GetGroupId(DateTime birthDate, int? gradeGroupAttributeId)
        {
            var timeDifference = System.DateTime.Now - birthDate;
            
            var ageYears = DateTime.Today.Year - birthDate.Year;
            if (DateTime.Today.Month < birthDate.Month || (DateTime.Today.Month == birthDate.Month && DateTime.Today.Day < birthDate.Day))
            {
                ageYears--;
            }

            // 0 to 1 year group
            if (ageYears == 0)
            {
                var ageMonths = DateTime.Today.Month - birthDate.Month;
                if (ageMonths < 0)
                {
                    ageMonths += 12;
                }

                // Jan birth months
                if (birthDate.Month == 1)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcJan0To1;
                        case 1:
                            return _applicationConfiguration.KcJan1To2;
                        case 2:
                            return _applicationConfiguration.KcJan2To3;
                        case 3:
                            return _applicationConfiguration.KcJan3To4;
                        case 4:
                            return _applicationConfiguration.KcJan4To5;
                        case 5:
                            return _applicationConfiguration.KcJan5To6;
                        case 6:
                            return _applicationConfiguration.KcJan6To7;
                        case 7:
                            return _applicationConfiguration.KcJan7To8;
                        case 8:
                            return _applicationConfiguration.KcJan8To9;
                        case 9:
                            return _applicationConfiguration.KcJan9To10;
                        case 10:
                            return _applicationConfiguration.KcJan10To11;
                        case 11:
                            return _applicationConfiguration.KcJan11To12;
                    }
                }

                // Feb birth months
                if (birthDate.Month == 2)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcFeb0To1;
                        case 1:
                            return _applicationConfiguration.KcFeb1To2;
                        case 2:
                            return _applicationConfiguration.KcFeb2To3;
                        case 3:
                            return _applicationConfiguration.KcFeb3To4;
                        case 4:
                            return _applicationConfiguration.KcFeb4To5;
                        case 5:
                            return _applicationConfiguration.KcFeb5To6;
                        case 6:
                            return _applicationConfiguration.KcFeb6To7;
                        case 7:
                            return _applicationConfiguration.KcFeb7To8;
                        case 8:
                            return _applicationConfiguration.KcFeb8To9;
                        case 9:
                            return _applicationConfiguration.KcFeb9To10;
                        case 10:
                            return _applicationConfiguration.KcFeb10To11;
                        case 11:
                            return _applicationConfiguration.KcFeb11To12;
                    }
                }

                // March birth months
                if (birthDate.Month == 3)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcMar0To1;
                        case 1:
                            return _applicationConfiguration.KcMar1To2;
                        case 2:
                            return _applicationConfiguration.KcMar2To3;
                        case 3:
                            return _applicationConfiguration.KcMar3To4;
                        case 4:
                            return _applicationConfiguration.KcMar4To5;
                        case 5:
                            return _applicationConfiguration.KcMar5To6;
                        case 6:
                            return _applicationConfiguration.KcMar6To7;
                        case 7:
                            return _applicationConfiguration.KcMar7To8;
                        case 8:
                            return _applicationConfiguration.KcMar8To9;
                        case 9:
                            return _applicationConfiguration.KcMar9To10;
                        case 10:
                            return _applicationConfiguration.KcMar10To11;
                        case 11:
                            return _applicationConfiguration.KcMar11To12;
                    }
                }

                // April birth months
                if (birthDate.Month == 4)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcApr0To1;
                        case 1:
                            return _applicationConfiguration.KcApr1To2;
                        case 2:
                            return _applicationConfiguration.KcApr2To3;
                        case 3:
                            return _applicationConfiguration.KcApr3To4;
                        case 4:
                            return _applicationConfiguration.KcApr4To5;
                        case 5:
                            return _applicationConfiguration.KcApr5To6;
                        case 6:
                            return _applicationConfiguration.KcApr6To7;
                        case 7:
                            return _applicationConfiguration.KcApr7To8;
                        case 8:
                            return _applicationConfiguration.KcApr8To9;
                        case 9:
                            return _applicationConfiguration.KcApr9To10;
                        case 10:
                            return _applicationConfiguration.KcApr10To11;
                        case 11:
                            return _applicationConfiguration.KcApr11To12;
                    }
                }

                // May birth months
                if (birthDate.Month == 5)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcMay0To1;
                        case 1:
                            return _applicationConfiguration.KcMay1To2;
                        case 2:
                            return _applicationConfiguration.KcMay2To3;
                        case 3:
                            return _applicationConfiguration.KcMay3To4;
                        case 4:
                            return _applicationConfiguration.KcMay4To5;
                        case 5:
                            return _applicationConfiguration.KcMay5To6;
                        case 6:
                            return _applicationConfiguration.KcMay6To7;
                        case 7:
                            return _applicationConfiguration.KcMay7To8;
                        case 8:
                            return _applicationConfiguration.KcMay8To9;
                        case 9:
                            return _applicationConfiguration.KcMay9To10;
                        case 10:
                            return _applicationConfiguration.KcMay10To11;
                        case 11:
                            return _applicationConfiguration.KcMay11To12;
                    }
                }

                // June birth months
                if (birthDate.Month == 6)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcJun0To1;
                        case 1:
                            return _applicationConfiguration.KcJun1To2;
                        case 2:
                            return _applicationConfiguration.KcJun2To3;
                        case 3:
                            return _applicationConfiguration.KcJun3To4;
                        case 4:
                            return _applicationConfiguration.KcJun4To5;
                        case 5:
                            return _applicationConfiguration.KcJun5To6;
                        case 6:
                            return _applicationConfiguration.KcJun6To7;
                        case 7:
                            return _applicationConfiguration.KcJun7To8;
                        case 8:
                            return _applicationConfiguration.KcJun8To9;
                        case 9:
                            return _applicationConfiguration.KcJun9To10;
                        case 10:
                            return _applicationConfiguration.KcJun10To11;
                        case 11:
                            return _applicationConfiguration.KcJun11To12;
                    }
                }

                // July birth months
                if (birthDate.Month == 7)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcJul0To1;
                        case 1:
                            return _applicationConfiguration.KcJul1To2;
                        case 2:
                            return _applicationConfiguration.KcJul2To3;
                        case 3:
                            return _applicationConfiguration.KcJul3To4;
                        case 4:
                            return _applicationConfiguration.KcJul4To5;
                        case 5:
                            return _applicationConfiguration.KcJul5To6;
                        case 6:
                            return _applicationConfiguration.KcJul6To7;
                        case 7:
                            return _applicationConfiguration.KcJul7To8;
                        case 8:
                            return _applicationConfiguration.KcJul8To9;
                        case 9:
                            return _applicationConfiguration.KcJul9To10;
                        case 10:
                            return _applicationConfiguration.KcJul10To11;
                        case 11:
                            return _applicationConfiguration.KcJul11To12;
                    }
                }

                // August birth months
                if (birthDate.Month == 8)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcAug0To1;
                        case 1:
                            return _applicationConfiguration.KcAug1To2;
                        case 2:
                            return _applicationConfiguration.KcAug2To3;
                        case 3:
                            return _applicationConfiguration.KcAug3To4;
                        case 4:
                            return _applicationConfiguration.KcAug4To5;
                        case 5:
                            return _applicationConfiguration.KcAug5To6;
                        case 6:
                            return _applicationConfiguration.KcAug6To7;
                        case 7:
                            return _applicationConfiguration.KcAug7To8;
                        case 8:
                            return _applicationConfiguration.KcAug8To9;
                        case 9:
                            return _applicationConfiguration.KcAug9To10;
                        case 10:
                            return _applicationConfiguration.KcAug10To11;
                        case 11:
                            return _applicationConfiguration.KcAug11To12;
                    }
                }

                // September birth months
                if (birthDate.Month == 9)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcSep0To1;
                        case 1:
                            return _applicationConfiguration.KcSep1To2;
                        case 2:
                            return _applicationConfiguration.KcSep2To3;
                        case 3:
                            return _applicationConfiguration.KcSep3To4;
                        case 4:
                            return _applicationConfiguration.KcSep4To5;
                        case 5:
                            return _applicationConfiguration.KcSep5To6;
                        case 6:
                            return _applicationConfiguration.KcSep6To7;
                        case 7:
                            return _applicationConfiguration.KcSep7To8;
                        case 8:
                            return _applicationConfiguration.KcSep8To9;
                        case 9:
                            return _applicationConfiguration.KcSep9To10;
                        case 10:
                            return _applicationConfiguration.KcSep10To11;
                        case 11:
                            return _applicationConfiguration.KcSep11To12;
                    }
                }

                // October birth months
                if (birthDate.Month == 10)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcOct0To1;
                        case 1:
                            return _applicationConfiguration.KcOct1To2;
                        case 2:
                            return _applicationConfiguration.KcOct2To3;
                        case 3:
                            return _applicationConfiguration.KcOct3To4;
                        case 4:
                            return _applicationConfiguration.KcOct4To5;
                        case 5:
                            return _applicationConfiguration.KcOct5To6;
                        case 6:
                            return _applicationConfiguration.KcOct6To7;
                        case 7:
                            return _applicationConfiguration.KcOct7To8;
                        case 8:
                            return _applicationConfiguration.KcOct8To9;
                        case 9:
                            return _applicationConfiguration.KcOct9To10;
                        case 10:
                            return _applicationConfiguration.KcOct10To11;
                        case 11:
                            return _applicationConfiguration.KcOct11To12;
                    }
                }

                // November birth months
                if (birthDate.Month == 11)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcNov0To1;
                        case 1:
                            return _applicationConfiguration.KcNov1To2;
                        case 2:
                            return _applicationConfiguration.KcNov2To3;
                        case 3:
                            return _applicationConfiguration.KcNov3To4;
                        case 4:
                            return _applicationConfiguration.KcNov4To5;
                        case 5:
                            return _applicationConfiguration.KcNov5To6;
                        case 6:
                            return _applicationConfiguration.KcNov6To7;
                        case 7:
                            return _applicationConfiguration.KcNov7To8;
                        case 8:
                            return _applicationConfiguration.KcNov8To9;
                        case 9:
                            return _applicationConfiguration.KcNov9To10;
                        case 10:
                            return _applicationConfiguration.KcNov10To11;
                        case 11:
                            return _applicationConfiguration.KcNov11To12;
                    }
                }

                // December birth months
                if (birthDate.Month == 12)
                {
                    switch (ageMonths)
                    {
                        case 0:
                            return _applicationConfiguration.KcDec0To1;
                        case 1:
                            return _applicationConfiguration.KcDec1To2;
                        case 2:
                            return _applicationConfiguration.KcDec2To3;
                        case 3:
                            return _applicationConfiguration.KcDec3To4;
                        case 4:
                            return _applicationConfiguration.KcDec4To5;
                        case 5:
                            return _applicationConfiguration.KcDec5To6;
                        case 6:
                            return _applicationConfiguration.KcDec6To7;
                        case 7:
                            return _applicationConfiguration.KcDec7To8;
                        case 8:
                            return _applicationConfiguration.KcDec8To9;
                        case 9:
                            return _applicationConfiguration.KcDec9To10;
                        case 10:
                            return _applicationConfiguration.KcDec10To11;
                        case 11:
                            return _applicationConfiguration.KcDec11To12;
                    }
                }
            }

            if (ageYears == 1)
            {
                switch (birthDate.Month)
                {
                    case 1:
                        return _applicationConfiguration.KcOneYearJan;
                    case 2:
                        return _applicationConfiguration.KcOneYearFeb;
                    case 3:
                        return _applicationConfiguration.KcOneYearMar;
                    case 4:
                        return _applicationConfiguration.KcOneYearApr;
                    case 5:
                        return _applicationConfiguration.KcOneYearMay;
                    case 6:
                        return _applicationConfiguration.KcOneYearJun;
                    case 7:
                        return _applicationConfiguration.KcOneYearJul;
                    case 8:
                        return _applicationConfiguration.KcOneYearAug;
                    case 9:
                        return _applicationConfiguration.KcOneYearSep;
                    case 10:
                        return _applicationConfiguration.KcOneYearOct;
                    case 11:
                        return _applicationConfiguration.KcOneYearNov;
                    case 12:
                        return _applicationConfiguration.KcOneYearDec;
                }
            }

            if (ageYears == 2)
            {
                switch (birthDate.Month)
                {
                    case 1:
                        return _applicationConfiguration.KcTwoYearJan;
                    case 2:
                        return _applicationConfiguration.KcTwoYearFeb;
                    case 3:
                        return _applicationConfiguration.KcTwoYearMar;
                    case 4:
                        return _applicationConfiguration.KcTwoYearApr;
                    case 5:
                        return _applicationConfiguration.KcTwoYearMay;
                    case 6:
                        return _applicationConfiguration.KcTwoYearJun;
                    case 7:
                        return _applicationConfiguration.KcTwoYearJul;
                    case 8:
                        return _applicationConfiguration.KcTwoYearAug;
                    case 9:
                        return _applicationConfiguration.KcTwoYearSep;
                    case 10:
                        return _applicationConfiguration.KcTwoYearOct;
                    case 11:
                        return _applicationConfiguration.KcTwoYearNov;
                    case 12:
                        return _applicationConfiguration.KcTwoYearDec;
                }
            }

            if (ageYears == 3)
            {
                switch (birthDate.Month)
                {
                    case 1:
                        return _applicationConfiguration.KcThreeYearJan;
                    case 2:
                        return _applicationConfiguration.KcThreeYearFeb;
                    case 3:
                        return _applicationConfiguration.KcThreeYearMar;
                    case 4:
                        return _applicationConfiguration.KcThreeYearApr;
                    case 5:
                        return _applicationConfiguration.KcThreeYearMay;
                    case 6:
                        return _applicationConfiguration.KcThreeYearJun;
                    case 7:
                        return _applicationConfiguration.KcThreeYearJul;
                    case 8:
                        return _applicationConfiguration.KcThreeYearAug;
                    case 9:
                        return _applicationConfiguration.KcThreeYearSep;
                    case 10:
                        return _applicationConfiguration.KcThreeYearOct;
                    case 11:
                        return _applicationConfiguration.KcThreeYearNov;
                    case 12:
                        return _applicationConfiguration.KcThreeYearDec;
                }
            }

            if (ageYears == 4)
            {
                switch (birthDate.Month)
                {
                    case 1:
                        return _applicationConfiguration.KcFourYearJan;
                    case 2:
                        return _applicationConfiguration.KcFourYearFeb;
                    case 3:
                        return _applicationConfiguration.KcFourYearMar;
                    case 4:
                        return _applicationConfiguration.KcFourYearApr;
                    case 5:
                        return _applicationConfiguration.KcFourYearMay;
                    case 6:
                        return _applicationConfiguration.KcFourYearJun;
                    case 7:
                        return _applicationConfiguration.KcFourYearJul;
                    case 8:
                        return _applicationConfiguration.KcFourYearAug;
                    case 9:
                        return _applicationConfiguration.KcFourYearSep;
                    case 10:
                        return _applicationConfiguration.KcFourYearOct;
                    case 11:
                        return _applicationConfiguration.KcFourYearNov;
                    case 12:
                        return _applicationConfiguration.KcFourYearDec;
                }
            }

            if (ageYears >= 5 && gradeGroupAttributeId == null)
            {
                switch (birthDate.Month)
                {
                    case 1:
                        return _applicationConfiguration.KcFiveYearJan;
                    case 2:
                        return _applicationConfiguration.KcFiveYearFeb;
                    case 3:
                        return _applicationConfiguration.KcFiveYearMar;
                    case 4:
                        return _applicationConfiguration.KcFiveYearApr;
                    case 5:
                        return _applicationConfiguration.KcFiveYearMay;
                    case 6:
                        return _applicationConfiguration.KcFiveYearJun;
                    case 7:
                        return _applicationConfiguration.KcFiveYearJul;
                    case 8:
                        return _applicationConfiguration.KcFiveYearAug;
                    case 9:
                        return _applicationConfiguration.KcFiveYearSep;
                    case 10:
                        return _applicationConfiguration.KcFiveYearOct;
                    case 11:
                        return _applicationConfiguration.KcFiveYearNov;
                    case 12:
                        return _applicationConfiguration.KcFiveYearDec;
                }
            }

            if (gradeGroupAttributeId != null)
            {
                switch (gradeGroupAttributeId)
                {
                    case 9032:
                        return _applicationConfiguration.KcKindergarten;
                    case 9033:
                        return _applicationConfiguration.KcFirstGrade;
                    case 9034:
                        return _applicationConfiguration.KcSecondGrade;
                    case 9035:
                        return _applicationConfiguration.KcThirdGrade;
                    case 9036:
                        return _applicationConfiguration.KcFourthGrade;
                    case 9037:
                        return _applicationConfiguration.KcFifthGrade;
                }
            }

            throw new Exception("Age Group Id Not Found for Birthday: " + birthDate);
        }
    }
}
