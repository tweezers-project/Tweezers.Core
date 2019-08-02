﻿using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Tweezers.DBConnector;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Api.Identity
{
    public static class IdentityManager
    {
        public const string SessionIdKey = "sessionId";
        public const string SessionExpiryKey = "sessionExpiry";

        public static bool UsingIdentity { get; private set; }

        public static void RegisterIdentity()
        {
            var usersSchema = CreateUsersSchema(true);

            if (TweezersSchemaFactory.Find(usersSchema.CollectionName, true) == null)
                TweezersSchemaFactory.AddObject(usersSchema);
            else
            {
                TweezersSchemaFactory.DeleteObject(usersSchema.CollectionName);
                TweezersSchemaFactory.AddObject(usersSchema);
            }

            UsingIdentity = true;
        }

        public static TweezersObject CreateUsersSchema(bool withInternals = false)
        {
            TweezersObject usersSchema = new TweezersObject()
            {
                CollectionName = "users",
                Internal = true,
            };

            usersSchema.DisplayNames.SingularName = "User";
            usersSchema.DisplayNames.PluralName = "Users";
            usersSchema.Icon = "person";

            usersSchema.Fields.Add("username", new TweezersField()
            {
                FieldProperties = new TweezersFieldProperties()
                {
                    FieldType = TweezersFieldType.String,
                    UiTitle = true,
                    Min = 1,
                    Max = 50,
                    Required = true,
                    Regex = @"[A-Za-z\d]+",
                    Name = "username",
                    DisplayName = "Username",
                }
            });

            if (withInternals)
            {
                usersSchema.Fields.Add("passwordHash", new TweezersField()
                {
                    FieldProperties = new TweezersFieldProperties()
                    {
                        FieldType = TweezersFieldType.String,
                        Required = true,
                        UiIgnore = true,
                        Name = "passwordHash",
                        DisplayName = "password",
                    }
                });

                usersSchema.Fields.Add(SessionIdKey, new TweezersField()
                {
                    FieldProperties = new TweezersFieldProperties()
                    {
                        FieldType = TweezersFieldType.String,
                        UiIgnore = true,
                        Name = SessionIdKey,
                        DisplayName = "Session ID",
                    }
                });

                usersSchema.Fields.Add(SessionExpiryKey, new TweezersField()
                {
                    FieldProperties = new TweezersFieldProperties()
                    {
                        FieldType = TweezersFieldType.String,
                        UiIgnore = true,
                        Name = SessionExpiryKey,
                        DisplayName = "SessionExpiry",
                    }
                });
            }

            return usersSchema;
        }

        public static JObject FindUserBySessionId(string sessionId)
        {
            FindOptions<JObject> sessionIdPredicate = new FindOptions<JObject>()
            {
                Predicate = (u) =>
                {
                    bool sessionIdEq = u[SessionIdKey].ToString().Equals(sessionId);
                    var sessionExpiryTime = long.Parse(u[SessionExpiryKey].ToString());
                    var now = DateTime.Now.ToFileTimeUtc();
                    bool sessionExpiryOk = sessionExpiryTime > now;
                    return sessionIdEq && sessionExpiryOk;
                },
                Take = 1,
            };

            TweezersObject usersObjectMetadata = TweezersSchemaFactory.Find("users", true);
            return usersObjectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, sessionIdPredicate)?.SingleOrDefault();
        }
    }
}
