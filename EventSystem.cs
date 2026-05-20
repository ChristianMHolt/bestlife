using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BitLifeClone
{
    public class EventSystem
    {
        private List<Event> _events;
        private Random _random;

        public EventSystem()
        {
            _events = new List<Event>();
            _random = new Random();
            LoadEvents();
        }

        private void LoadEvents()
        {
            try
            {
                if (File.Exists("events.json"))
                {
                    string json = File.ReadAllText("events.json");
                    var loadedEvents = JsonSerializer.Deserialize<List<Event>>(json);
                    if (loadedEvents != null)
                    {
                        _events = loadedEvents;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading events: {ex.Message}");
            }
        }

        public Event? GetRandomEvent()
        {
            if (_events.Count == 0) return null;

            // 20% chance for an event each year
            if (_random.Next(0, 100) < 20)
            {
                int index = _random.Next(0, _events.Count);
                return _events[index];
            }

            return null;
        }
    }

    public class Event
    {
        public string PromptText { get; set; } = string.Empty;
        public List<EventChoice> Choices { get; set; } = new List<EventChoice>();
    }

    public class EventChoice
    {
        public string Text { get; set; } = string.Empty;
        public Dictionary<string, int> StatChanges { get; set; } = new Dictionary<string, int>();
    }
}
