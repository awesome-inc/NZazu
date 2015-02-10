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

namespace Sample.Samples
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    class BehaviorSample : IHaveSample
    {
        public INZazuSample Sample { get; private set; }

        public int Order { get { return 40; } }

        public BehaviorSample()
        {
            // register behavior
            BehaviorExtender.Register("OpenUrlOnStringEnter", typeof(OpenUrlOnStringEnterBehavior));

            // create sample
            Sample = new NZazuSampleViewModel
            {
                Name = "Behavior",
                Description = "",
                FormDefinition = new FormDefinition
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
                            Description = "describe your impression of the weather",
                            Checks = new []
                            {
                                new CheckDefinition { Type = "required" }
                            },
                            Behavior = new BehaviorDefinition { Name = "OpenUrlOnStringEnter" },
                        }
                    }
                },
                FormData = new Dictionary<string, string>
                {
                    { "comment", "type in a url like http://google.de and open it with STRG+Enter" }, 
                }
            };
        }

        #region behavior and behavior test

        private class OpenUrlOnStringEnterBehavior : INZazuWpfFieldBehavior
        {
            private Control _control;
            private KeyEventHandler _handler;

            public void AttachTo(Control valueControl)
            {
                if (valueControl == null) throw new ArgumentNullException("valueControl");

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