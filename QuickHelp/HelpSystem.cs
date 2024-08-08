﻿using System;
using System.Collections.Generic;

namespace QuickHelp;

/// <summary>
/// Manages multiple cross-referenced help databases.
/// </summary>
public class HelpSystem
{
    /// <summary>
    /// Gets the collection of help databases in this library.
    /// </summary>
    public List<HelpDatabase> Databases { get; } = [];

    /// <summary>
    /// Finds a database with the given name, ignoring case.
    /// </summary>
    /// <param name="name">Name of the database to find.</param>
    /// <returns>The help database, or null if not found.</returns>
    public HelpDatabase FindDatabase(string name)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        foreach (var database in Databases)
        {
            if (database.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                return database;
        }
        return null;
    }

    /// <summary>
    /// Resolves the given uri in the current help system.
    /// </summary>
    /// <param name="referrer"></param>
    /// <param name="uri"></param>
    /// <returns>
    /// The topic pointed to by the uri, or <c>null</c> if one cannot be
    /// found.
    /// </returns>
    public HelpTopic ResolveUri(HelpDatabase referrer, HelpUri uri)
    {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        HelpUriType uriType = uri.Type;
        if (uriType == HelpUriType.LocalTopic)
        {
            if (referrer != null)
            {
                int topicIndex = uri.TopicIndex;
                if (topicIndex >= 0 && topicIndex < referrer.Topics.Count)
                    return referrer.Topics[topicIndex];
            }
        }
        else if (uriType == HelpUriType.GlobalContext)
        {
            var dbName = uri.DatabaseName;
            var db = FindDatabase(dbName);
            if (db != null)
                return db.ResolveContext(uri.ContextString);
        }
        else if (uriType == HelpUriType.LocalContext)
        {
            if (referrer != null)
                return referrer.ResolveContext(uri.ContextString);
        }
        else if (uriType == HelpUriType.Context)
        {
            if (referrer != null)
            {
                var topic = referrer.ResolveContext(uri.ContextString);
                if (topic != null)
                    return topic;
            }
            foreach (var db in Databases)
            {
                var topic = db.ResolveContext(uri.ContextString);
                if (topic != null)
                    return topic;
            }
            return null;
        }
        return null;
    }
}
