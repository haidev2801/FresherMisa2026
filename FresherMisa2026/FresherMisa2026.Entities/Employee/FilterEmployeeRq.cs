using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Employee
{
    public class FilterEmployeeRq
    {
        //Lọc theo phòng ban
        public Guid? DepartmentID { get; set; }

       
        // Lọc theo vị trí
        public Guid? PositionID { get; set; }


        // Lọc lương từ
        public decimal? SalaryFrom { get; set; }


        // Lọc lương đến
        public decimal? SalaryTo { get; set; }

        // 0: Nam, 1: Nữ, 2: Khác
        public int? Gender { get; set; }

  
        //Ngày vào làm từ
        public DateTime? HireDateFrom { get; set; }


        // Ngày vào làm đến
        public DateTime? HireDateTo { get; set; }
    }
}
