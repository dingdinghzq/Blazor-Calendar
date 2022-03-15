﻿using BlazorCalendar.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorCalendar;

partial class AnnualView : CalendarBase
{
    private int _months = 11;
    [Parameter]
    public int Months
    {
        get
        {
            return _months;
        }
        set
        {
            _months = value;
            if (_months > 1) _months--;
        }
    }

    private DateTime _firstdate;
    [CascadingParameter(Name = "FirstDate")]
    public DateTime FirstDate
    {
        get
        {
            if (_firstdate == DateTime.MinValue) _firstdate = DateTime.Today;
            return _firstdate.Date;
        }
        set
        {
            _firstdate = value;
        }
    }
    
    [CascadingParameter(Name = "TasksList")]
    public Tasks[]? TasksList { get; set; }

    [Parameter]
    public string WeekDaysColor { get; set; } = "#FFF";

    [Parameter]
    public string SaturdayColor { get; set; } = "#DBE4F2";

    [Parameter]
    public string SundayColor { get; set; } = "#CBD9ED";

    [Parameter]
    public PriorityLabel PriorityDisplay { get; set; } = PriorityLabel.Code;

    [Parameter]
    public EventCallback<ClickTaskParameter> TaskClick { get; set; }

    [Parameter]
    public EventCallback<ClickEmptyDayParameter> EmptyDayClick { get; set; }

    [Parameter]
    public bool Draggable { get; set; } = false;

    [Parameter]
    public EventCallback<DragDropParameter> DragStart { get; set; }

    [Parameter]
    public EventCallback<DragDropParameter> DropTask { get; set; }

    private DateTime m = DateTime.Today;
    private DateTime day = default;
    private Tasks? TaskDragged;

    private async Task ClickInternal(MouseEventArgs e, DateTime day)
    {
        if (day == default) return;

        // There can be several tasks in one day :
        List<int> listID = new();
        if (TasksList != null )
        {
            for (var k = 0; k < TasksList.Length; k++)
            {
                Tasks t = TasksList[k];

                if (t.DateStart.Date <= day.Date && day.Date <= t.DateEnd.Date)
                {
                    listID.Add(t.ID);
                }
            }
        }

        if (listID.Count > 0)
        {
            ClickTaskParameter clickTaskParameter = new()
            {
                IDList = listID,
                X = e.ClientX,
                Y = e.ClientY
            };

            await TaskClick.InvokeAsync(clickTaskParameter);
        }
        else
        {
            ClickEmptyDayParameter clickEmptyDayParameter = new()
            {
                Day = day,
                X = e.ClientX,
                Y = e.ClientY
            };

            await EmptyDayClick.InvokeAsync(clickEmptyDayParameter);
        }
    }

    private async Task HandleDragStart(DateTime day, int taskID) 
    {
        TaskDragged = new Tasks()
        {
            ID = taskID
        };

        DragDropParameter dragDropParameter = new()
        {
            Day = day,
            taskID = TaskDragged.ID
        };

        await DragStart.InvokeAsync(dragDropParameter);
    }

    private async Task HandleDayOnDrop(DateTime day)
    {
        if (TaskDragged == null) return;

        DragDropParameter dragDropParameter = new()
        {
            Day = day,
            taskID = TaskDragged.ID
        };

        await DropTask.InvokeAsync(dragDropParameter);
    }
}