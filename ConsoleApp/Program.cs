using System;
using Challenge;
using Challenge.DataContracts;
using ConsoleApp;
using Task = System.Threading.Tasks.Task;


// Данное приложение можно запускать под Windows, Linux, Mac.
// Для запуска приложения необходимо скачать и установить .NET 8.
// Скачать можно тут: https://dotnet.microsoft.com/download/dotnet


const string teamSecret = "d3Zi71XCyiJGYmnW19hZaRtH8sbWl7M"; // Вставь сюда ключ команды
if (string.IsNullOrEmpty(teamSecret))
{
    Console.WriteLine("Задай секрет своей команды, чтобы можно было делать запросы от ее имени");
    Console.ReadLine();
    return;
}

var challengeClient = new ChallengeClient(teamSecret);

const string challengeId = "git-course";
Console.WriteLine($"Нажми ВВОД, чтобы получить информацию о соревновании {challengeId}");
Console.ReadLine();
Console.WriteLine("Ожидание...");
var challenge = await challengeClient.GetChallengeAsync(challengeId);
Console.WriteLine(challenge.Description);
Console.WriteLine();
Console.WriteLine("----------------");
Console.WriteLine();

const string taskType = "steganography";

var utcNow = DateTime.UtcNow;
string currentRound = null;
foreach (var round in challenge.Rounds)
{
    if (round.StartTimestamp < utcNow && utcNow < round.EndTimestamp)
        currentRound = round.Id;
}

Console.WriteLine($"Нажми ВВОД, чтобы получить первые 50 взятых командой задач типа {taskType} в раунде {currentRound}");
Console.ReadLine();
Console.WriteLine("Ожидание...");
var firstTasks = await challengeClient.GetTasksAsync(currentRound, taskType, TaskStatus.Pending, 0, 1);
for (int i = 0; i < firstTasks.Count; i++)
{
    var task = firstTasks[i];
    Console.WriteLine($"  Задание {i + 1}, статус {task.Status}");
    Console.WriteLine($"  Формулировка: {task.UserHint}");
    Console.WriteLine($"                {task.Question}");
    Console.WriteLine();
}
Console.WriteLine("----------------");
Console.WriteLine();

Console.WriteLine($"Нажми ВВОД, чтобы получить задачу типа {taskType} в раунде {currentRound}");
Console.ReadLine();
Console.WriteLine("Ожидание...");
var newTasks = await challengeClient.GetTasksAsync(currentRound, taskType, TaskStatus.Pending, 0, 50);
foreach (var newTask in newTasks)
{
    var answer = Solver.Solve(newTask);
    var updatedTask = await challengeClient.CheckTaskAnswerAsync(newTask.Id, answer);
    
    if (updatedTask.Status == TaskStatus.Success)
        Console.WriteLine($"Ура! Ответ угадан!");
    else if (updatedTask.Status == TaskStatus.Failed)
        Console.WriteLine($"Похоже ответ не подошел и задачу больше сдать нельзя...");
    Console.WriteLine();
    Console.WriteLine("----------------");
    Console.WriteLine();

    Console.WriteLine($"Нажми ВВОД, чтобы завершить работу программы");

}