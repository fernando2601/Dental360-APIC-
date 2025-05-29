using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ClinicApi.Models;
using ClinicApi.Services;

namespace ClinicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LearningAreaController : ControllerBase
    {
        private readonly ILearningAreaService _learningAreaService;

        public LearningAreaController(ILearningAreaService learningAreaService)
        {
            _learningAreaService = learningAreaService;
        }

        // Cursos
        [HttpGet("courses")]
        public async Task<ActionResult> GetCourses(
            [FromQuery] string? category = null,
            [FromQuery] string? level = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 25)
        {
            var courses = await _learningAreaService.GetCoursesAsync(category, level, page, limit);
            return Ok(courses);
        }

        [HttpGet("courses/{id}")]
        public async Task<ActionResult> GetCourse(int id)
        {
            var course = await _learningAreaService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            return Ok(course);
        }

        [HttpPost("courses")]
        public async Task<ActionResult> CreateCourse([FromBody] CreateCourseDto courseDto)
        {
            var course = await _learningAreaService.CreateCourseAsync(courseDto);
            return Ok(course);
        }

        // Módulos
        [HttpGet("courses/{courseId}/modules")]
        public async Task<ActionResult> GetCourseModules(int courseId)
        {
            var modules = await _learningAreaService.GetCourseModulesAsync(courseId);
            return Ok(modules);
        }

        [HttpPost("courses/{courseId}/modules")]
        public async Task<ActionResult> CreateModule(int courseId, [FromBody] CreateModuleDto moduleDto)
        {
            var module = await _learningAreaService.CreateModuleAsync(courseId, moduleDto);
            return Ok(module);
        }

        // Lições
        [HttpGet("modules/{moduleId}/lessons")]
        public async Task<ActionResult> GetModuleLessons(int moduleId)
        {
            var lessons = await _learningAreaService.GetModuleLessonsAsync(moduleId);
            return Ok(lessons);
        }

        [HttpPost("modules/{moduleId}/lessons")]
        public async Task<ActionResult> CreateLesson(int moduleId, [FromBody] CreateLessonDto lessonDto)
        {
            var lesson = await _learningAreaService.CreateLessonAsync(moduleId, lessonDto);
            return Ok(lesson);
        }

        // Progresso
        [HttpGet("progress")]
        public async Task<ActionResult> GetUserProgress()
        {
            var progress = await _learningAreaService.GetUserProgressAsync();
            return Ok(progress);
        }

        [HttpPost("lessons/{lessonId}/complete")]
        public async Task<ActionResult> CompleteLesson(int lessonId)
        {
            var result = await _learningAreaService.CompleteLessonAsync(lessonId);
            return Ok(result);
        }

        // Certificados
        [HttpGet("certificates")]
        public async Task<ActionResult> GetCertificates()
        {
            var certificates = await _learningAreaService.GetCertificatesAsync();
            return Ok(certificates);
        }

        [HttpPost("courses/{courseId}/generate-certificate")]
        public async Task<ActionResult> GenerateCertificate(int courseId)
        {
            var certificate = await _learningAreaService.GenerateCertificateAsync(courseId);
            return Ok(certificate);
        }
    }
}