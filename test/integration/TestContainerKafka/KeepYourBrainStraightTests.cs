using System;
using Xunit;
namespace TestContainerKafka;

public class ListNode
{
    public int Value;
    public ListNode Next;

    public ListNode(int value = 0, ListNode next = null)
    {
        Value = value;
        Next = next;
    }
}

public static class LinkedListReverser
{
    /// <summary>
    /// Reverses a singly linked list.
    /// </summary>
    /// <param name="head">The head node of the linked list.</param>
    /// <returns>The new head node of the reversed linked list.</returns>
    public static ListNode ReverseList(ListNode head)
    {
        ListNode previous = null; // Tracks the previous node (initially null)
        ListNode current = head; // Start with the head of the list

        while (current != null)
        {
            // Save the next node before breaking the link
            ListNode nextTemp = current.Next;

            // Reverse the current node's pointer
            current.Next = previous;

            // Move the pointers one step forward
            previous = current;
            current = nextTemp;
        }

        // Return the new head (previous now points to the last node)
        return previous;
    }
}

public class LinkedListReverserTests
{
    /// <summary>
    /// Helper method to convert an array to a linked list.
    /// </summary>
    private ListNode ConvertArrayToLinkedList(int[] values)
    {
        if (values == null || values.Length == 0) return null;

        var head = new ListNode(values[0]);
        var current = head;

        for (int i = 1; i < values.Length; i++)
        {
            current.Next = new ListNode(values[i]);
            current = current.Next;
        }

        return head;
    }

    /// <summary>
    /// Helper method to convert a linked list to an array.
    /// </summary>
    private int[] ConvertLinkedListToArray(ListNode head)
    {
        var result = new System.Collections.Generic.List<int>();

        while (head != null)
        {
            result.Add(head.Value);
            head = head.Next;
        }

        return result.ToArray();
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3, 4, 5 }, new int[] { 5, 4, 3, 2, 1 })]
    [InlineData(new int[] { 1, 2 }, new int[] { 2, 1 })]
    [InlineData(new int[] { 1 }, new int[] { 1 })]
    [InlineData(new int[] { }, new int[] { })]
    public void ReverseList_ShouldReverseLinkedList(int[] input, int[] expected)
    {
        // Arrange
        var head = ConvertArrayToLinkedList(input);

        // Act
        var reversedHead = LinkedListReverser.ReverseList(head);

        // Assert
        var result = ConvertLinkedListToArray(reversedHead);
        Assert.Equal(expected, result);
    }
}