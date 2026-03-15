using Mullai.TUI.TUI.State;
using Terminal.Gui.Views;
using Mullai.Abstractions.Observability;
using Terminal.Gui.Input;

namespace Mullai.TUI.TUI.Views;

/// <summary>
/// A version of ChatHistoryView that uses Terminal.Gui.TextView for better text handling.
/// </summary>
public class ChatHistoryView : TextView
{
    private const int RightMargin = 2;
    private const int LeftMargin = 1;

    public ChatHistoryView()
    {
        ReadOnly = true;
        WordWrap = true;
        CanFocus = true; // TextView usually needs focus to allow scrolling/selection
        // MouseBindings.Add (MouseFlags.RightButtonClicked, Command.Copy);
    }

    private string _cachedHistoryText = string.Empty;
    private int _cachedEntryCount = 0;

    /// <summary>Recompute the full text from the current state snapshot.</summary>
    public void UpdateMessages(
        IEnumerable<object> entries,
        string streamingBuffer,
        bool isThinking)
    {
        var entryList = entries.ToList();
        
        // If the number of messages hasn't changed, we can reuse the cached history
        // (assuming messages are immutable except for the very last one being streamed)
        if (entryList.Count != _cachedEntryCount)
        {
            var sbHistory = new System.Text.StringBuilder();
            // We render all but the last message if the last one is being updated
            // Actually, let's render all except the streaming buffer / thinking part
            foreach (var entry in entryList)
            {
                if (entry is ChatMessage msg)
                {
                    // If this is the last message and it matches the streaming buffer, 
                    // we handle it separately below to allow for rapid updates.
                    // But for simplicity in this TUI, we'll cache everything except the "active" tail.
                }
                RenderEntry(sbHistory, entry);
            }
            _cachedHistoryText = sbHistory.ToString();
            _cachedEntryCount = entryList.Count;
        }

        var sbFinal = new System.Text.StringBuilder(_cachedHistoryText);

        if (isThinking)
        {
            sbFinal.AppendLine(" Mullai");
            var text = string.IsNullOrEmpty(streamingBuffer) ? " ● Thinking…" : $" {streamingBuffer}";
            sbFinal.AppendLine(text);
        }

        Text = sbFinal.ToString();
        ScrollVertical(-1);
    }

    private void RenderEntry(System.Text.StringBuilder sb, object entry)
    {
        if (entry is ChatMessage msg)
        {
            var sender = msg.IsUser ? "You" : "Mullai";
            sb.AppendLine($" {sender}");
            sb.AppendLine($" {msg.Content}");
            sb.AppendLine();
        }
        else if (entry is ToolCallObservation tool)
        {
            sb.AppendLine($" [Tool: {tool.ToolName}]");
            if (tool.Succeeded)
            {
                sb.AppendLine($" Result: {tool.Result}");
            }
            else
            {
                sb.AppendLine($" Error: {tool.Error}");
            }
            sb.AppendLine();
        }
    }
    
    protected override bool OnMouseEvent(Mouse mouse)
    {
        if (mouse.Flags.HasFlag(MouseFlags.RightButtonClicked))
        {
            mouse.Handled = true;
            Copy();
            return false;
        }
        
        return base.OnMouseEvent(mouse);
    }
}
