using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Challenge;
using Challenge.DataContracts;
using ConsoleApp;
using TaskStatus = Challenge.DataContracts.TaskStatus;

const string teamSecret = "d3Zi71XCyiJGYmnW19hZaRtH8sbWl7M";

if (string.IsNullOrEmpty(teamSecret))
{
    Console.WriteLine("Не задан ключ команды");
    return;
}

var challengeClient = new ChallengeClient(teamSecret);

const string challengeId = "git-course";

Console.WriteLine("Получение информации о соревновании...");
var challenge = await challengeClient.GetChallengeAsync(challengeId);

var utcNow = DateTime.UtcNow;

var currentRound = challenge.Rounds
    .FirstOrDefault(x =>
        x.StartTimestamp < utcNow &&
        utcNow < x.EndTimestamp)
    ?.Id;

if (currentRound == null)
{
    Console.WriteLine("Нет активного раунда");
    return;
}

Console.WriteLine($"Активный раунд: {currentRound}");
Console.WriteLine();

Console.WriteLine("Выберите режим:");
Console.WriteLine("1 - Получать новые задачи и решать");
Console.WriteLine("2 - Решать только принятые задачи");

Console.Write("> ");

var mode = Console.ReadLine() == "2"
    ? RunMode.SolvePending
    : RunMode.AcceptAndSolve;

var supportedTypes = new HashSet<string>
{
    "polynomial-root",
    "steganography",
    "math",
    "determinant",
    "cypher"
};

var solved = 0;
var failed = 0;


while (true)
    try
    {
        List<TaskResponse> tasks = new();

        if (mode == RunMode.AcceptAndSolve)
            tasks.Add(
                await challengeClient.AskNewTaskAsync(currentRound));
        else
            foreach (var type in supportedTypes)
            {
                var taskList = await challengeClient.GetTasksAsync(
                    currentRound,
                    type,
                    TaskStatus.Pending);

                tasks.AddRange(taskList);
            }

        foreach (var task in tasks)
        {
            Console.WriteLine();
            Console.WriteLine("----------------");
            Console.WriteLine($"Тип: {task.TypeId}");
            Console.WriteLine($"ID: {task.Id}");

            if (!supportedTypes.Contains(task.TypeId))
            {
                Console.WriteLine("Тип задачи не поддерживается");
                continue;
            }

            try
            {
                var answer = Solver.Solve(task);

                Console.WriteLine($"Ответ: {answer}");


                var result =
                    await challengeClient.CheckTaskAnswerAsync(
                        task.Id,
                        answer);


                if (result.Status == TaskStatus.Success)
                {
                    solved++;
                    Console.WriteLine("Ответ принят");
                }
                else
                {
                    failed++;
                    Console.WriteLine(
                        $"Ответ отклонен: {result.Status}");
                }


                Console.WriteLine(
                    $"Решено: {solved}, ошибок: {failed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Ошибка решения: {ex.Message}");
            }
        }


        if (mode == RunMode.SolvePending) await Task.Delay(3000);
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"Ошибка запроса: {ex.Message}");

        await Task.Delay(3000);
    }

internal enum RunMode
{
    AcceptAndSolve = 1,
    SolvePending = 2
}