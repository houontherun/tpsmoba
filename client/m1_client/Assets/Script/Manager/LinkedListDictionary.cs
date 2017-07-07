﻿/********************************************************************************
** auth： yanwei
** date： 2016-08-08
** desc： 频繁用到增加与删除少，但根据id查询对象与遍历对象都很多的情形。这个类提供对这种应用场景具有较好性能的容器实现。
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

  public sealed class LinkedListDictionary<TKey,TValue>
  {
    public bool Contains(TKey id)
    {
      return kLinkNodeDictionary.ContainsKey(id);
    }
    ///这里不考虑重复，外界调用时保证（性能考虑）
    public void AddFirst(TKey id, TValue obj)
    {
      LinkedListNode<TValue> linkNode = kObjects.AddFirst(obj);
      if (null != linkNode) {
        kLinkNodeDictionary.Add(id, linkNode);
      }
    }
    public void AddLast(TKey id, TValue obj)
    {
      LinkedListNode<TValue> linkNode = kObjects.AddLast(obj);
      if (null != linkNode) {
        kLinkNodeDictionary.Add(id, linkNode);
      }
    }
    public void Remove(TKey id)
    {
      if (kLinkNodeDictionary.ContainsKey(id)) {
        LinkedListNode<TValue> linkNode = kLinkNodeDictionary[id];
        kLinkNodeDictionary.Remove(id);
        try { kObjects.Remove(linkNode); } catch (Exception) { }
      }
    }
    public void Clear()
    {
      kLinkNodeDictionary.Clear();
      kObjects.Clear();
    }
    public bool TryGetValue(TKey id, out TValue value)
    {
      LinkedListNode<TValue> linkNode;
      bool ret = kLinkNodeDictionary.TryGetValue(id, out linkNode);
      if (ret) {
        value = linkNode.Value;
      } else {
        value = default(TValue);
      }
      return ret;
    }
    public int Count
    {
      get
      {
        return kLinkNodeDictionary.Count;
      }
    }
    public TValue this[TKey id]
    {
      get
      {
        TValue ret;
        if (Contains(id)) {
          LinkedListNode<TValue> linkNode = kLinkNodeDictionary[id];
          ret = linkNode.Value;
        } else {
          ret = default(TValue);
        }
        return ret;
      }
      set
      {
        if (Contains(id)) {
          LinkedListNode<TValue> linkNode = kLinkNodeDictionary[id];
          linkNode.Value = value;
        } else {
          AddLast(id, value);
        }
      }
    }
    public LinkedListNode<TValue> FirstValue
    {
      get
      {
        return kObjects.First;
      }
    }
    public LinkedListNode<TValue> LastValue
    {
      get
      {
        return kObjects.Last;
      }
    }
    public void CopyValuesTo(TValue[] array,int index)
    {
      kObjects.CopyTo(array, index);
    }

    private Dictionary<TKey, LinkedListNode<TValue>> kLinkNodeDictionary = new Dictionary<TKey, LinkedListNode<TValue>>();
    private LinkedList<TValue> kObjects = new LinkedList<TValue>();
  }
