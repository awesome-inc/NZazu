using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using FluentAssertions;
using NUnit.Framework;
using NZazu;
using NZazu.Contracts;
using NZazu.FieldBehavior;

namespace NZazuFiddle.Samples
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class BehaviorSample : SampleBase
    {
        public BehaviorSample()
            : base(40)
        {
            // register behavior
            BehaviorExtender.Register<OpenUrlOnStringEnterBehavior>("OpenUrlOnStringEnter");

            Sample = new SampleViewModel
            {
                Name = "Behavior",
                Fiddle = ToFiddle(new FormDefinition
                {
                    Fields = new[]
                    {
                        new FieldDefinition
                        {
                            Key = "headercomment",
                            Prompt = "",
                            Type = "label",
                            Description = "in the dialog below you can enter a text and open a url by hitting STRG+Return"
                        },
                        new FieldDefinition
                        {
                            Key = "comment",
                            Type = "string",
                            Prompt = "Comment",
                            Hint = "",
                            Settings = new Dictionary<string, string>{{"Height", "100"}},
                            Description = "describe your impression of the weather",
                            Checks = new []
                            {
                                new CheckDefinition { Type = "required" }
                            },
                            Behavior = new BehaviorDefinition { Name = "OpenUrlOnStringEnter" },
                        },
                        new FieldDefinition
                        {
                            Key ="group",
                            Type = "group",
                            Fields = new []
                            {
                                new FieldDefinition
                                {
                                    Key = "nested.comment", Type ="string",
                                    Behavior = new BehaviorDefinition { Name = "OpenUrlOnStringEnter" }
                                }
                            }
                        }
                    }
                },
                new Dictionary<string, string>
                {
                    { "comment", "type in a url like http://google.de and open it with Ctrl+Enter" }, 
                    { "nested.comment", "type in a url like http://google.de and open it with Ctrl+Enter" }, 
                })
            };
        }

        #region behavior and behavior test

        // ReSharper disable once ClassNeverInstantiated.Local
        private class OpenUrlOnStringEnterBehavior : INZazuWpfFieldBehavior
        {
            private Control _control;
            private KeyEventHandler _handler;

            public void AttachTo(INZazuWpfField field, INZazuWpfView view)
            {
                if (field == null) throw new ArgumentNullException("field");
                var valueControl = field.ValueControl;

                _control = valueControl;

                _handler = HandleKeyUp;
                _control.KeyUp += _handler;
            }

            public void Detach()
            {
                if (_control == null) return;

                _control.KeyUp -= _handler;
                _handler = null;
                _control = null;
            }

            private void HandleKeyUp(object sender, KeyEventArgs e)
            {
                if (!((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.Return))) return;

                var link = GetLinkAtPosition(((TextBox)_control).Text, ((TextBox)_control).SelectionStart);
                if (link == null) return;

                Process.Start(link);
            }

            internal static string GetLinkAtPosition(string text, int position)
            {
                var spaceBefore = text.Substring(0, position).LastIndexOf(" ", StringComparison.Ordinal);
                if (spaceBefore < 0) spaceBefore = 0;
                else spaceBefore += 1;

                var spaceAfter = text.Substring(position).IndexOf(" ", StringComparison.Ordinal);
                if (spaceAfter < 0) spaceAfter = text.Length;
                else spaceAfter += position;

                var length = spaceAfter - spaceBefore;
                if (length <= 10) return null;
                var textUnderCursor = text.Substring(spaceBefore, length);
                return textUnderCursor.StartsWith("http://") ? textUnderCursor : null;
            }
        }

        [TestFixture]
        // ReSharper disable once InconsistentNaming
        class OpenUrlOnStringEnterBehavior_Should
        {
            [Test]
            [TestCase("asdfklj asöfkdljsa fdöakfjl saöljad fösadfa", 6, null)]
            [TestCase("asdfklj asöfkdljsa http://google.de fdöakfjl saöljad fösadfa", 3, null)]
            [TestCase("asdfklj asöfkdljsa ftp://google.de fdöakfjl saöljad fösadfa", 25, null)]
            [TestCase("asdfklj asöfkdljsa http://google.de fdöakfjl saöljad fösadfa", 25, "http://google.de")]
            [TestCase("http://google.de asdfklj asöfkdljsa fdöakfjl saöljad fösadfa", 4, "http://google.de")]
            [TestCase("http://google.de", 0, "http://google.de")]
            [TestCase("asdfklj asöfkdljsa fdöakfjl saöljad fösadfa http://google.de", 56, "http://google.de")]
            public void Extract_Url(string text, int position, string expected)
            {
                var url = OpenUrlOnStringEnterBehavior.GetLinkAtPosition(text, position);

                url.Should().Be(expected);
            }
        }

        #endregion

    }
}