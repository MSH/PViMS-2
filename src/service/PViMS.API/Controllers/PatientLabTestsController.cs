using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize]
    public class PatientLabTestsController : ControllerBase
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientLabTest> _patientLabTestRepository;
        private readonly IRepositoryInt<LabTest> _labTestRepository;
        private readonly IRepositoryInt<LabTestUnit> _labTestUnitRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientLabTestsController(
            IMapper mapper,
            IUrlHelper urlHelper,
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientLabTest> patientLabTestRepository,
            IRepositoryInt<LabTest> labTestRepository,
            IRepositoryInt<LabTestUnit> labTestUnitRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientLabTestRepository = patientLabTestRepository ?? throw new ArgumentNullException(nameof(patientLabTestRepository));
            _labTestRepository = labTestRepository ?? throw new ArgumentNullException(nameof(labTestRepository));
            _labTestUnitRepository = labTestUnitRepository ?? throw new ArgumentNullException(nameof(labTestUnitRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Get a single patient lab test using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient lab test</param>
        /// <returns>An ActionResult of type PatientLabTestIdentifierDto</returns>
        [HttpGet("{patientId}/labtests/{id}", Name = "GetPatientLabTestByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientLabTestIdentifierDto>> GetPatientLabTestByIdentifier(long patientId, long id)
        {
            var mappedPatientLabTest = await GetPatientLabTestAsync<PatientLabTestIdentifierDto>(patientId, id);
            if (mappedPatientLabTest == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientLabTest<PatientLabTestIdentifierDto>(mappedPatientLabTest));
        }

        /// <summary>
        /// Get a single patient lab test using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient labTest</param>
        /// <returns>An ActionResult of type PatientLabTestDetailDto</returns>
        [HttpGet("{patientId}/labtests/{id}", Name = "GetPatientLabTestByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientLabTestDetailDto>> GetPatientLabTestByDetail(long patientId, long id)
        {
            var mappedPatientLabTest = await GetPatientLabTestAsync<PatientLabTestDetailDto>(patientId, id);
            if (mappedPatientLabTest == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientLabTest<PatientLabTestDetailDto>(CustomPatientLabTestMap(mappedPatientLabTest)));
        }

        /// <summary>
        /// Create a new patient lab test record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="labTestForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/labtests", Name = "CreatePatientLabTest")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientLabTest(int patientId, 
            [FromBody] PatientLabTestForUpdateDto labTestForUpdate)
        {
            if (labTestForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new labTest");
                return BadRequest(ModelState);
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var labTestFromRepo = _labTestRepository.Get(lt => lt.Description == labTestForUpdate.LabTest);
            if (labTestFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate lab test");
            }

            LabTestUnit labTestUnitFromRepo = null;
            if(!String.IsNullOrWhiteSpace(labTestForUpdate.TestUnit))
            {
                labTestUnitFromRepo = _labTestUnitRepository.Get(u => u.Description == labTestForUpdate.TestUnit);
                if (labTestUnitFromRepo == null)
                {
                    ModelState.AddModelError("Message", "Unable to locate lab test unit");
                }
            }

            ValidateLabTestForUpdateModel(patientFromRepo, labTestForUpdate);

            if (ModelState.IsValid)
            {
                var labTestDetail = PrepareLabTestDetail(labTestForUpdate);
                if (!labTestDetail.IsValid())
                {
                    labTestDetail.InvalidAttributes.ForEach(element => ModelState.AddModelError("Message", element));
                }

                if (ModelState.IsValid)
                {
                    var patientLabTest = new PatientLabTest
                    {
                        LabTest = labTestFromRepo,
                        TestDate = labTestForUpdate.TestDate,
                        TestResult = labTestForUpdate.TestResultCoded,
                        LabValue = labTestForUpdate.TestResultValue,
                        TestUnit = labTestUnitFromRepo,
                        ReferenceLower = labTestForUpdate.ReferenceLower,
                        ReferenceUpper = labTestForUpdate.ReferenceUpper,
                        Patient = patientFromRepo
                    };

                    _modelExtensionBuilder.UpdateExtendable(patientLabTest, labTestDetail.CustomAttributes, "Admin");

                    _patientLabTestRepository.Save(patientLabTest);
                    _unitOfWork.Complete();

                    var mappedPatientLabTest = _mapper.Map<PatientLabTestIdentifierDto>(patientLabTest);
                    if (mappedPatientLabTest == null)
                    {
                        return StatusCode(500, "Unable to locate newly added lab test");
                    }

                    return CreatedAtRoute("GetPatientLabTestByIdentifier",
                        new
                        {
                            id = mappedPatientLabTest.Id
                        }, CreateLinksForPatientLabTest<PatientLabTestIdentifierDto>(mappedPatientLabTest));
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing patient lab test
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the lab test</param>
        /// <param name="labTestForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/labTests/{id}", Name = "UpdatePatientLabTest")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientLabTest(long patientId, long id,
            [FromBody] PatientLabTestForUpdateDto labTestForUpdate)
        {
            if (labTestForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var patientLabTestFromRepo = await _patientLabTestRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id);
            if (patientLabTestFromRepo == null)
            {
                return NotFound();
            }

            var labTestFromRepo = _labTestRepository.Get(lt => lt.Description == labTestForUpdate.LabTest);
            if (labTestFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate lab test");
            }

            LabTestUnit labTestUnitFromRepo = null;
            if (!String.IsNullOrWhiteSpace(labTestForUpdate.TestUnit))
            {
                labTestUnitFromRepo = _labTestUnitRepository.Get(u => u.Description == labTestForUpdate.TestUnit);
                if (labTestUnitFromRepo == null)
                {
                    ModelState.AddModelError("Message", "Unable to locate lab test unit");
                }
            }

            ValidateLabTestForUpdateModel(patientFromRepo, labTestForUpdate);

            if (ModelState.IsValid)
            {
                var labTestDetail = PrepareLabTestDetail(labTestForUpdate);
                if (!labTestDetail.IsValid())
                {
                    labTestDetail.InvalidAttributes.ForEach(element => ModelState.AddModelError("Message", element));
                }

                if (ModelState.IsValid)
                {
                    patientLabTestFromRepo.TestDate = labTestForUpdate.TestDate;
                    patientLabTestFromRepo.TestResult = labTestForUpdate.TestResultCoded;
                    patientLabTestFromRepo.LabValue = labTestForUpdate.TestResultValue;
                    patientLabTestFromRepo.TestUnit = labTestUnitFromRepo;
                    patientLabTestFromRepo.ReferenceLower = labTestForUpdate.ReferenceLower;
                    patientLabTestFromRepo.ReferenceUpper = labTestForUpdate.ReferenceUpper;

                    _modelExtensionBuilder.UpdateExtendable(patientLabTestFromRepo, labTestDetail.CustomAttributes, "Admin");

                    _patientLabTestRepository.Update(patientLabTestFromRepo);
                    _unitOfWork.Complete();

                    return Ok();
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing patient lab test
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the lab test</param>
        /// <param name="labTestForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/labTests/{id}/archive", Name = "ArchivePatientLabTest")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientLabTest(long patientId, long id,
            [FromBody] ArchiveDto labTestForDelete)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var patientLabTestFromRepo = await _patientLabTestRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id);
            if (patientLabTestFromRepo == null)
            {
                return NotFound();
            }
            if (patientLabTestFromRepo.LabTest == null)
            {
                ModelState.AddModelError("Message", "Lab test not included");
            }

            if (Regex.Matches(labTestForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < labTestForDelete.Reason.Length)
            {
                ModelState.AddModelError("Message", "Reason contains invalid characters (Enter A-Z, a-z, space, period, apostrophe)");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError("Message", "Unable to locate user");
            }

            if (ModelState.IsValid)
            {
                patientLabTestFromRepo.Archived = true;
                patientLabTestFromRepo.ArchivedDate = DateTime.Now;
                patientLabTestFromRepo.ArchivedReason = labTestForDelete.Reason;
                patientLabTestFromRepo.AuditUser = user;
                _patientLabTestRepository.Update(patientLabTestFromRepo);
                _unitOfWork.Complete();
            }

            return Ok();
        }

        /// <summary>
        /// Get single patient lab test from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Parent resource id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPatientLabTestAsync<T>(long patientId, long id) where T : class
        {
            var patientLabTestFromRepo = await _patientLabTestRepository.GetAsync(pc => pc.Patient.Id == patientId && pc.Id == id);

            if (patientLabTestFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatientLabTest = _mapper.Map<T>(patientLabTestFromRepo);

                return mappedPatientLabTest;
            }

            return null;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientLabTestDetailDto CustomPatientLabTestMap(PatientLabTestDetailDto dto)
        {
            var patientLabTest = _patientLabTestRepository.Get(p => p.Id == dto.Id);
            if (patientLabTest == null)
            {
                return dto;
            }
            IExtendable patientLabTestExtended = patientLabTest;

            // Map all custom attributes
            dto.LabTestAttributes = _modelExtensionBuilder.BuildModelExtension(patientLabTestExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = (h.Type == CustomAttributeType.Selection) ? GetSelectionValue(h.AttributeKey, h.Value.ToString()) : string.Empty
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            return dto;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientLabTestIdentifierDto CreateLinksForPatientLabTest<T>(T dto)
        {
            PatientLabTestIdentifierDto identifier = (PatientLabTestIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "PatientLabTest", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        /// Validate the input model for updating a lab test
        /// </summary>
        private void ValidateLabTestForUpdateModel(Patient patientFromRepo, PatientLabTestForUpdateDto labTestForUpdateDto)
        {
            if (labTestForUpdateDto.TestDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Test Date should be before current date");
            }
            if (labTestForUpdateDto.TestDate < patientFromRepo.DateOfBirth)
            {
                ModelState.AddModelError("Message", "Test Date should be after Date Of Birth");
            }

            if (Regex.Matches(labTestForUpdateDto.TestResultValue, @"[-a-zA-Z0-9 .]").Count < labTestForUpdateDto.TestResultValue.Length)
            {
                ModelState.AddModelError("Message", "Comments contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period)");
            }

            if (Regex.Matches(labTestForUpdateDto.ReferenceLower, @"[-a-zA-Z0-9 .]").Count < labTestForUpdateDto.ReferenceLower.Length)
            {
                ModelState.AddModelError("Message", "Comments contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period)");
            }

            if (Regex.Matches(labTestForUpdateDto.ReferenceUpper, @"[-a-zA-Z0-9 .]").Count < labTestForUpdateDto.ReferenceUpper.Length)
            {
                ModelState.AddModelError("Message", "Comments contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period)");
            }
        }

        /// <summary>
        /// Prepare the model for the lab test
        /// </summary>
        private LabTestDetail PrepareLabTestDetail(PatientLabTestForUpdateDto labTestForUpdate)
        {
            var labTestDetail = new LabTestDetail();
            labTestDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientLabTest>();

            foreach (var newAttribute in labTestForUpdate.Attributes)
            {
                var customAttribute = _customAttributeRepository.Get(ca => ca.Id == newAttribute.Key);
                if (customAttribute != null)
                {
                // Validate attribute exists for household entity and is a PMT attribute
                var attributeDetail = labTestDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);
                    
                if (attributeDetail == null)
                    {
                        ModelState.AddModelError("Message", $"Unable to locate custom attribute on patient labTest {newAttribute.Key}");
                    }
                    else
                    {
                        attributeDetail.Value = newAttribute.Value;
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", $"Unable to locate custom attribute {newAttribute.Key}");
                }
            }
            // Update patient custom attributes from source
            return labTestDetail;
        }

        /// <summary>
        /// Get the corresponding selection value
        /// </summary>
        /// <param name="attributeKey">The custom attribute key look up value</param>
        /// <param name="selectionKey">The selection key look up value</param>
        /// <returns></returns>
        private string GetSelectionValue(string attributeKey, string selectionKey)
        {
            var selectionitem = _selectionDataItemRepository.Get(s => s.AttributeKey == attributeKey && s.SelectionKey == selectionKey);

            return (selectionitem == null) ? string.Empty : selectionitem.Value;
        }
    }
}
