using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveMeAloneFuncSkillForge.Functional
{
    public class TaskRiskResult
    {
        public double RiskScore { get; set; }
        public string RiskCategory { get; set; }
    }

    public class TaskRiskEvaluator
    {
        public static TaskRiskResult EvaluateRisk(TaskData task) =>
            task.Fork(
                t => t.EstimatedHours * t.ComplexityLevel, // eforrt score
                t => (t.DueDate - DateTime.Now).TotalDays, // time left
                (effort, daysRemaining) =>
                {
                    var riskScore = effort / Math.Max(daysRemaining, 1);
                    var category = riskScore > 100 ? "High" :
                                   riskScore > 50 ? "Medium" : "Low";

                    return new TaskRiskResult
                    {
                        RiskScore = riskScore,
                        RiskCategory = category
                    };
                });
    }
}
