using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SystemMonitor.TestUtilities
{
    public class EventsWaiter
    {
        private static readonly TimeSpan EventsRegistrationMaxTime = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan EventsProsecutionMaxTime = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan WaitingTimeBetweenRetries = TimeSpan.FromMilliseconds(100);

        public static async Task WaitForEventsRegistrationAsync(StringWriter stringWriter)
        {
            await WaitForExpectedOutputAsync(
                stringWriter,
                "Monitoring directory",
                EventsRegistrationMaxTime);
        }

        public static async Task WaitForEventsProsecutionAsync(
            StringWriter stringWriter,
            IEnumerable<string>? expectedChangedFiles = null,
            IEnumerable<string>? expectedCreatedFiles = null,
            IEnumerable<string>? expectedNotCreatedFiles = null,
            IEnumerable<string>? expectedDeletedFiles = null,
            IEnumerable<(string OldPath, string NewPath)>? expectedRenamedFiles = null)
        {
            List<string> expectedOutput = [];
            List<string> notExpectedOutput = [];

            if (expectedChangedFiles is not null)
            {
                foreach (string expectedChangedFile in expectedChangedFiles)
                {
                    expectedOutput.Add($"Changed: {expectedChangedFile}");
                }
            }

            if (expectedCreatedFiles is not null)
            {
                foreach (string expectedCreatedFile in expectedCreatedFiles)
                {
                    expectedOutput.Add($"Created: {expectedCreatedFile}");
                }
            }

            if (expectedNotCreatedFiles is not null)
            {
                foreach (string expectedNotCreatedFile in expectedNotCreatedFiles)
                {
                    notExpectedOutput.Add($"Created: {expectedNotCreatedFile}");
                }
            }

            if (expectedDeletedFiles is not null)
            {
                foreach (string expectedDeletedFile in expectedDeletedFiles)
                {
                    expectedOutput.Add($"Deleted: {expectedDeletedFile}");
                }
            }

            if (expectedRenamedFiles is not null)
            {
                foreach ((string OldPath, string NewPath) in expectedRenamedFiles)
                {
                    expectedOutput.Add($"Renamed: {OldPath} to {NewPath}");
                }
            }

            await WaitForExpectedOutputAsync(
                stringWriter,
                expectedOutput,
                notExpectedOutput,
                EventsProsecutionMaxTime);
        }

        private static async Task WaitForExpectedOutputAsync(
            StringWriter stringWriter, string expectedOutput, TimeSpan maxWaitingTime)
        {
            using CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(maxWaitingTime);

            bool expectedOutputPrinted = false;

            do
            {
                try
                {
                    stringWriter.ToString().Should().Contain(expectedOutput);

                    expectedOutputPrinted = true;
                }
                catch
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        throw;
                    }

                    await Task.Delay(WaitingTimeBetweenRetries);
                }
            } while (!expectedOutputPrinted);
        }

        private static async Task WaitForExpectedOutputAsync(
            StringWriter stringWriter,
            IEnumerable<string> expectedOutput,
            IEnumerable<string> notExpectedOutput,
            TimeSpan maxWaitingTime)
        {
            using CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(maxWaitingTime);

            bool expectedOutputPrinted = false;

            List<string> expectedOutputList = expectedOutput.ToList();
            List<string> notExpectedOutputList = notExpectedOutput.ToList();

            do
            {
                try
                {
                    foreach (string expectedOutputPart in expectedOutputList)
                    {
                        stringWriter.ToString().Should().Contain(expectedOutputPart);
                    }

                    foreach (string expectedOutputPart in notExpectedOutputList)
                    {
                        stringWriter.ToString().Should().NotContain(expectedOutputPart);
                    }

                    expectedOutputPrinted = true;
                }
                catch
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        throw;
                    }

                    await Task.Delay(WaitingTimeBetweenRetries);
                }
            } while (!expectedOutputPrinted);
        }
    }
}