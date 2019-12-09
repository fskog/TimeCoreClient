using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeCoreClient
{
    public class AppState
    {
        public event Action OnChange;

        private List<TimeEntry> _timeEntries;
        public TimeEntry CurrentTimeEntry;

        private readonly Blazored.LocalStorage.ILocalStorageService _localStorage;
        private readonly string TimeEntriesKey;

        private bool _localStorageInitComplete = false;

        public AppState(Blazored.LocalStorage.ILocalStorageService localStorage)
        {
           TimeEntriesKey = "timeEntries";
            _localStorage = localStorage;
            _timeEntries = new List<TimeEntry>();
            
            NotifyStateChanged();
        }


        private async Task LoadTimeEntriesFromLocalStorage()
        {
            _timeEntries = await _localStorage.GetItemAsync<List<TimeEntry>>(TimeEntriesKey) ?? new List<TimeEntry>();
        }

        private async void SaveTimeEntriesToLocalStorage()
        {
            await _localStorage.SetItemAsync(TimeEntriesKey, _timeEntries);
        }

        public async Task<List<TimeEntry>> GetTimeEntries()
        {
            if(!_localStorageInitComplete)
            {
                await LoadTimeEntriesFromLocalStorage();
            }
            return _timeEntries;
        }

        public void AddOrUpdateTime(TimeEntry Time)
        {
            Console.WriteLine($"----------AddOrUpdateTime:---------");
            foreach (var t in _timeEntries)
            {
                Console.WriteLine($"TimeList entry: {System.Text.Json.JsonSerializer.Serialize(t)}");
            }

            var existingTime = _timeEntries.FirstOrDefault(x => x.SystemId == Time.SystemId);
            Console.WriteLine($"Old time: {System.Text.Json.JsonSerializer.Serialize(existingTime)}");
            Console.WriteLine($"New time: {System.Text.Json.JsonSerializer.Serialize(Time)}");
            if (existingTime != null)
            {
                Console.WriteLine($"AddOrUpdateTime: Existing entry found, updating entry:");
                Console.WriteLine($"Old time: {System.Text.Json.JsonSerializer.Serialize(existingTime)}");
                Console.WriteLine($"New time: {System.Text.Json.JsonSerializer.Serialize(Time)}");
                existingTime = new TimeEntry()
                {
                    SystemId = Time.SystemId,
                    Title = string.IsNullOrWhiteSpace(Time.Title) ? "(no description)" : Time.Title,
                    StartTime = Time.StartTime,
                    EndTime = Time.EndTime
                };
            }
            else
            {
                Console.WriteLine($"AddOrUpdateTime: No existing entry found, adding new entry:");
                Console.WriteLine($"New time: {System.Text.Json.JsonSerializer.Serialize(Time)}");
                _timeEntries.Add(new TimeEntry() { 
                    SystemId = Time.SystemId,
                    Title = string.IsNullOrWhiteSpace(Time.Title) ? "(no description)" : Time.Title,
                    StartTime = Time.StartTime,
                    EndTime = Time.EndTime
                });
            }
            Console.WriteLine($"AppState has changed. Invoke OnChange event.");
            SaveTimeEntriesToLocalStorage();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

    }
}
