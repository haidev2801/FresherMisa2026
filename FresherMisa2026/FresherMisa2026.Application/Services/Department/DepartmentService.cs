using FresherMisa2026.Application.Extensions;
using FresherMisa2026.Application.Interfaces;
using FresherMisa2026.Application.Interfaces.Repositories;
using FresherMisa2026.Application.Interfaces.Services;
using FresherMisa2026.Application.Services;
using FresherMisa2026.Entities.Department;
using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Application.Services
{
    public class DepartmentService : BaseService<Department>, IDepartmentSerice
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(
            IBaseRepository<Department> baseRepository,
            IDepartmentRepository departmentRepository
            ) : base(baseRepository)
        {
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// Lấy department theo code
        /// </summary>
        /// <returns></returns>
        /// Created By: dvhai (10/04/2026)
        public async Task<Department> GetDepartmentByCodeAsync(string code)
        {
            var department = await _departmentRepository.GetDepartmentByCode(code);
            if (department == null)
                throw new Exception("department is null");

            return department;
        }

        #region OVERRIDE METHODS
        protected override async Task<bool> ValidateBeforeDelete(Guid entityId)
        {
            //1. Validate còn nhân viên trong phòng ban không
            bool hasEmployee = true;

            return !hasEmployee;
        }
        #endregion OVERRIDE METHODS
    }
}
