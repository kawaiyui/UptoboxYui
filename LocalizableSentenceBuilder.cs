using CommandLine;
using CommandLine.Text;

namespace UptoboxYui
{
    /// <summary>
    /// 命令行插件本地化
    /// </summary>
    public class LocalizableSentenceBuilder : SentenceBuilder
    {
        public override Func<string> RequiredWord
        {
            get { return () => Resource.SentenceRequiredWord; }
        }

        public override Func<string> ErrorsHeadingText
        {
            // Cannot be pluralized
            get { return () => Resource.SentenceErrorsHeadingText; }
        }

        public override Func<string> UsageHeadingText
        {
            get { return () => Resource.SentenceUsageHeadingText; }
        }

        public override Func<bool, string> HelpCommandText
        {
            get
            {
                return isOption => isOption
                    ? Resource.SentenceHelpCommandTextOption
                    : Resource.SentenceHelpCommandTextVerb;
            }
        }

        public override Func<bool, string> VersionCommandText
        {
            get { return _ => Resource.SentenceVersionCommandText; }
        }

        public override Func<Error, string> FormatError
        {
            get
            {
                return error =>
                {
                    switch (error.Tag)
                    {
                        case ErrorType.BadFormatTokenError:
                            return String.Format(Resource.SentenceBadFormatTokenError, ((BadFormatTokenError)error).Token);
                        case ErrorType.MissingValueOptionError:
                            return String.Format(Resource.SentenceMissingValueOptionError, ((MissingValueOptionError)error).NameInfo.NameText);
                        case ErrorType.UnknownOptionError:
                            return String.Format(Resource.SentenceUnknownOptionError, ((UnknownOptionError)error).Token);
                        case ErrorType.MissingRequiredOptionError:
                            var errMisssing = ((MissingRequiredOptionError)error);
                            return errMisssing.NameInfo.Equals(NameInfo.EmptyName)
                                       ? Resource.SentenceMissingRequiredOptionError
                                       : String.Format(Resource.SentenceMissingRequiredOptionError, errMisssing.NameInfo.NameText);
                        case ErrorType.BadFormatConversionError:
                            var badFormat = ((BadFormatConversionError)error);
                            return badFormat.NameInfo.Equals(NameInfo.EmptyName)
                                       ? Resource.SentenceBadFormatConversionErrorValue
                                       : String.Format(Resource.SentenceBadFormatConversionErrorOption, badFormat.NameInfo.NameText);
                        case ErrorType.SequenceOutOfRangeError:
                            var seqOutRange = ((SequenceOutOfRangeError)error);
                            return seqOutRange.NameInfo.Equals(NameInfo.EmptyName)
                                       ? Resource.SentenceSequenceOutOfRangeErrorValue
                                       : String.Format(Resource.SentenceSequenceOutOfRangeErrorOption,
                                            seqOutRange.NameInfo.NameText);
                        case ErrorType.BadVerbSelectedError:
                            return String.Format(Resource.SentenceBadVerbSelectedError, ((BadVerbSelectedError)error).Token);
                        case ErrorType.NoVerbSelectedError:
                            return Resource.SentenceNoVerbSelectedError;
                        case ErrorType.RepeatedOptionError:
                            return String.Format(Resource.SentenceRepeatedOptionError, ((RepeatedOptionError)error).NameInfo.NameText);
                        case ErrorType.SetValueExceptionError:
                            var setValueError = (SetValueExceptionError)error;
                            return String.Format(Resource.SentenceSetValueExceptionError, setValueError.NameInfo.NameText, setValueError.Exception.Message);
                    }
                    throw new InvalidOperationException();
                };
            }
        }

        public override Func<IEnumerable<MutuallyExclusiveSetError>, string> FormatMutuallyExclusiveSetErrors
        {
            get
            {
                return errors =>
                {
                    var bySet = from e in errors
                                group e by e.SetName into g
                                select new { SetName = g.Key, Errors = g.ToList() };

                    var msgs = bySet.Select(
                        set =>
                        {
                            var names = String.Join(
                                String.Empty,
                                (from e in set.Errors select String.Format("'{0}', ", e.NameInfo.NameText)).ToArray());
                            var namesCount = set.Errors.Count();

                            var incompat = String.Join(
                                String.Empty,
                                (from x in
                                     (from s in bySet where !s.SetName.Equals(set.SetName) from e in s.Errors select e)
                                    .Distinct()
                                 select String.Format("'{0}', ", x.NameInfo.NameText)).ToArray());
                            //TODO: Pluralize by namesCount
                            return
                                String.Format(Resource.SentenceMutuallyExclusiveSetErrors,
                                    names.Substring(0, names.Length - 2), incompat.Substring(0, incompat.Length - 2));
                        }).ToArray();
                    return string.Join(Environment.NewLine, msgs);
                };
            }
        }

        public override Func<string> OptionGroupWord
        {
            get { return () => Resource.SentenceOptionGroupWord; }
        }
    }
}