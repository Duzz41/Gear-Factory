using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    private List<Clockwork_AI> robots = new List<Clockwork_AI>();
    private void Start()
    {
        // Sahnedeki tüm BaseRobotAI nesnelerini bul ve listeye ekle
        robots.AddRange(FindObjectsOfType<Clockwork_AI>());

        // Her bir robota başlangıç görevi ata veya kontrol et
        foreach (Clockwork_AI robot in robots)
        {
            // Eğer robot bozuksa, tamir et
            if (robot.currentState == RobotState.Broken)
            {
                robot.Repair();
            }

            // Robot köylü durumuna geçtiyse, ona bir görev ata (örneğin savaşçı)
            if (robot.currentState == RobotState.Villager)
            {
                AssignJob(robot, RobotState.Warrior);
            }
        }
    }
    public void AssignJob(Clockwork_AI robot, RobotState job)
    {
        if (robot.currentState == RobotState.Villager)
        {
            robot.ChangeState(job);
            Debug.Log("Robot yeni göreve atandı: " + job.ToString());
        }
        else
        {
            Debug.Log("Bu robot şu anda köylü değil, ona görev verilemez.");
        }
    }
    
}
