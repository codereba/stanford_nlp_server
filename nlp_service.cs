using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using java.util;
using java.io;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.ie.crf;
using edu.stanford.nlp.ie.machinereading;
using edu.stanford.nlp.process;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.trees;
using edu.stanford.nlp.parser.lexparser;
using edu.stanford.nlp.tagger.maxent;
using edu.stanford.nlp.time;
using edu.stanford.nlp.util;
using Console = System.Console;


namespace standfordnlp
{
    class nlp_service
    {
        private enum language_type
        {
            LANGUAGE_EN,
            LANGUAGE_CN,
            LANGUAGE_AR,
        };

        //private enum nlp_model_type
        //{
        //    NER_MODEL,
        //    POS_TAGGER_MODEL,
        //    DEPENDENCY_PARSE_MODEL,
        //    SEGMENTER_MODEL, 
        //};

        private MaxentTagger pos_tagger_cn;
        private MaxentTagger pos_tagger_en;
        private CRFClassifier ner_classifier_en;
        private LexicalizedParser nlp_parser_cn;
        private AnnotationPipeline pos_pipeline;
        StanfordCoreNLP nlp_pipeline;
 
        private CRFClassifier segmenter;

        static string jarRoot; 
        
        public nlp_service()
        {
            jarRoot = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            jarRoot += @"data\";
        }

        int init()
        {
            int ret = 0; 

            do
            {
                //var _jarRoot = jarRoot;
                //_jarRoot += @"\stanford-postagger-full-2016-10-31";
                //var modelsDirectory = _jarRoot + @"\models";

                //Console.WriteLine("initializing pos tagger\n"); 

                //// Loading POS Tagger
                //this.pos_tagger_cn = new MaxentTagger(modelsDirectory + @"\chinese-distsim.tagger" ); //@"\wsj-0-18-bidirectional-nodistsim.tagger");
                //if (null == this.pos_tagger_cn)
                //{
                //    ret = -1; 
                //    break;
                //}

                // Path to the folder with classifies models
                //_jarRoot = jarRoot;
                //_jarRoot += @"\stanford-ner-2016-10-31";
                //var classifiersDirecrory = _jarRoot + @"\classifiers";

                //Console.WriteLine("initializing ner\n"); 

                //// Loading 3 class classifier model
                //ner_classifier_en = CRFClassifier.getClassifierNoExceptions(
                //    classifiersDirecrory + @"\english.all.3class.distsim.crf.ser.gz");
                //if( null == ner_classifier_en)
                //{
                //    ret = -1;
                //    break; 
                //}

                // Path to models extracted from `stanford-parser-3.6.0-models.jar`
                var _jarRoot = jarRoot;
                var modelsDirectory = _jarRoot;

                Console.WriteLine("initializing dependency parser\n"); 

                // Loading english PCFG parser from file
                nlp_parser_cn = LexicalizedParser.loadModel(modelsDirectory + @"\chinesePCFG.ser.gz"); // englishPCFG.ser.gz");
                if (null == nlp_parser_cn)
                {
                    ret = -1;
                    break;
                }

                // Path to the folder with models extracted from `stanford-corenlp-3.7.0-models.jar`
                //_jarRoot = jarRoot;
                //_jarRoot += @"\stanford-corenlp-full-2016-10-31\models";
                //modelsDirectory = _jarRoot + @"\edu\stanford\nlp\models";

                //Console.WriteLine("initializing pos tagger\n"); 

                // Annotation pipeline configuration
                //pos_pipeline = new AnnotationPipeline();
                //pos_pipeline.addAnnotator(new TokenizerAnnotator(false));
                //pos_pipeline.addAnnotator(new WordsToSentencesAnnotator(false));

                //// Loading POS Tagger and including them into pipeline
                //pos_tagger_en = new MaxentTagger(modelsDirectory +
                //                              @"\pos-tagger\english-left3words\english-left3words-distsim.tagger");
                //pos_pipeline.addAnnotator(new POSTaggerAnnotator(pos_tagger_en));

                //if (null == pos_tagger_en)
                //{
                //    ret = -1;
                //    break;
                //}


                // Annotation pipeline configuration
                var props = new Properties();
                //props.setProperty("annotators", "tokenize, ssplit, pos, lemma, parse, ner,dcoref");
                //props.setProperty("ner.useSUTime", "0");

                //nlp_pipeline = new StanfordCoreNLP(props); 

                var segmenterData = jarRoot;

                // Setup Segmenter loading properties
                props.clear(); 

                props.setProperty("sighanCorporaDict", segmenterData);
                // Lines below are needed because CTBSegDocumentIteratorFactory accesses it
                props.setProperty("serDictionary", segmenterData + @"\dict-chris6.ser.gz");
                //props.setProperty("testFile", sampleData);
                props.setProperty("readStdIn", "");
                props.setProperty("inputEncoding", "UTF-8");
                props.setProperty("sighanPostProcessing", "true");

                Console.WriteLine("initializing segmenter\n"); 

                // Load Word Segmenter
                segmenter = new CRFClassifier(props);
                segmenter.loadClassifierNoExceptions(segmenterData + @"\ctb.gz", props);
            } while (false);
            return ret;
        }

        int pos_tagger( string text_input, ref string text_output )
        {
            int ret = 0;

            do
            {
                ret = -1;
                break; 

                if( null == pos_tagger_cn )
                {
                    ret = -1; 
                    break; 
                }
                // Text for tagging
                //var text = "A Part-Of-Speech Tagger (POS Tagger) is a piece of software that reads text in some language "
                //           + "and assigns parts of speech to each word (and other token), such as noun, verb, adjective, etc., although "
                //           + "generally computational applications use more fine-grained POS tags like 'noun-plural'.";

                var sentences = MaxentTagger.tokenizeText(new java.io.StringReader(text_input)).toArray();
                foreach (ArrayList sentence in sentences)
                {
                    var taggedSentence = pos_tagger_cn.tagSentence(sentence);
                    text_output = SentenceUtils.listToString(taggedSentence, false); 
                    Console.WriteLine(SentenceUtils.listToString(taggedSentence, false));
                }
            } while (false);
            return ret; 
        }

        int ner( string text_input, ref string text_output )
        {
            int ret = 0;
            do
            {
                if (null == ner_classifier_en)
                {
                    ret = -1;
                    break;
                }
                var s1 = "Good afternoon Rajat Raina, how are you today?";
                Console.WriteLine("{0}\n", ner_classifier_en.classifyToString(s1));

                var s2 = "I go to school at Stanford University, which is located in California.";
                Console.WriteLine("{0}\n", ner_classifier_en.classifyWithInlineXML(s2));

                Console.WriteLine("{0}\n", ner_classifier_en.classifyToString(s2, "xml", true));

            } while (false);
            return ret;
        }

        int parser(string text_input, ref string text_output)
        {
            int ret = 0;

            do
            {
                text_output = ""; 

                if( text_input.Length == 0 )
                {
                    ret = -1;
                    break; 
                }

                // This sample shows parsing a list of correctly tokenized words
                //var sent = new[] { "This", "is", "an", "easy", "sentence", "." };
                //var rawWords = SentenceUtils.toCoreLabelList(sent);
                //var tree = lp.apply(rawWords);
                //tree.pennPrint();

                // This option shows loading and using an explicit tokenizer
                //var sent2 = "This is another sentence.";
                var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
                var sent2Reader = new java.io.StringReader(text_input);
                var rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
                sent2Reader.close();
                var tree2 = nlp_parser_cn.apply(rawWords2);

                // Extract dependencies from lexical tree
                //var tlp = new PennTreebankLanguagePack(); //ChineseTreebankLanguagePack();
                //try
                //{
                //    var gsf = tlp.grammaticalStructureFactory();
                //    var gs = gsf.newGrammaticalStructure(tree2);
                //    var tdl = gs.typedDependenciesCCprocessed();

                //    Console.WriteLine("\n{0}\n", tdl);
                //}
                //finally
                //{ }

                PrintWriter string_print;
                java.io.StringWriter string_writer;
                string_writer = new java.io.StringWriter();
                string_print = new PrintWriter(string_writer);

                // Extract collapsed dependencies from parsed tree
                var tp = new edu.stanford.nlp.trees.TreePrint("penn"); // "wordsAndTags,penn"); // typedDependenciesCollapsed");
                tp.printTree(tree2, string_print);
                text_output = string_writer.toString();  ; 
            } while (false);
            return ret;
        }

        int segment(string text_input, ref string text_output )
        {
            int ret = 0;
            do
            {
                if (text_input.Length == 0)
                {
                    ret = -1;
                    break;
                }

                // Path to the folder with models
                //var sampleData = jarRoot;
                //sampleData += @"\stanford-segmenter-2016-10-31\test.simp.utf8";

                // `test.simple.utf8` contains following text:
                // 面对新世纪，世界各国人民的共同愿望是：继续发展人类以往创造的一切文明成果，克服20世纪困扰着人类的战争和贫
                // 困问题，推进和平与发展的崇高事业，创造一个美好的世界。

                // This is a very simple demo of calling the Chinese Word Segmenter programmatically.
                // It assumes an input file in UTF8. This will run correctly in the distribution home
                // directory. To run in general, the properties for where to find dictionaries or
                // normalizations have to be set.
                // @author Christopher Manning

                //segmenter.classifyAndWriteAnswers(sampleData);
                text_output = segmenter.classifyToString(text_input);
            } while (false);
            return ret;
        }

        int su_time( string text_input, ref string text_output )
        {
            int ret = -1; 
            var _jarRoot = jarRoot;
            _jarRoot += @"\stanford-corenlp-full-2016-10-31\models";
            var modelsDirectory = _jarRoot + @"\edu\stanford\nlp\models";

            // SUTime configuration
            var sutimeRules = modelsDirectory + @"\sutime\defs.sutime.txt,"
                              + modelsDirectory + @"\sutime\english.holidays.sutime.txt,"
                              + modelsDirectory + @"\sutime\english.sutime.txt";
            var props = new Properties();
            props.setProperty("sutime.rules", sutimeRules);
            props.setProperty("sutime.binders", "0");
            pos_pipeline.addAnnotator(new TimeAnnotator("sutime", props));

            // Sample text for time expression extraction
            //var text = "Three interesting dates are 18 Feb 1997, the 20th of july and 4 days from today.";
            var annotation = new Annotation(text_input);
            annotation.set(new CoreAnnotations.DocDateAnnotation().getClass(), "2013-07-14");
            pos_pipeline.annotate(annotation);

            Console.WriteLine("{0}\n", annotation.get(new CoreAnnotations.TextAnnotation().getClass()));

            var timexAnnsAll = annotation.get(new TimeAnnotations.TimexAnnotations().getClass()) as ArrayList;
            foreach (CoreMap cm in timexAnnsAll)
            {
                var tokens = cm.get(new CoreAnnotations.TokensAnnotation().getClass()) as List;
                var first = tokens.get(0);
                var last = tokens.get(tokens.size() - 1);
                var time = cm.get(new TimeExpression.Annotation().getClass()) as TimeExpression;
                Console.WriteLine("{0} [from char offset {1} to {2}] --> {3}", cm, first, last, time.getTemporal());
            }
            return ret; 
        }

        int core_nlp( string text_input, ref string text_output)
        {
            int ret = 0;

            do
            {
                text_output = ""; 

                if( text_input.Length == 0)
                {
                    break;
                }

                // Path to the folder with models extracted from `stanford-corenlp-3.7.0-models.jar`
                var _jarRoot = jarRoot;
                _jarRoot += @"\stanford-corenlp-full-2016-10-31\models";

                // Text for processing
                //var text = "Kosgi Santosh sent an email to Stanford University. He didn't get a reply.";

                // We should change current directory, so StanfordCoreNLP could find all the model files automatically
                var current_dir = Environment.CurrentDirectory;
                Directory.SetCurrentDirectory(_jarRoot);
                Directory.SetCurrentDirectory(current_dir);

                // Annotation
                var annotation = new Annotation(text_input);
                nlp_pipeline.annotate(annotation);

                // Result - Pretty Print
                using (var stream = new ByteArrayOutputStream())
                {
                    nlp_pipeline.prettyPrint(annotation, new PrintWriter(stream));
                    Console.WriteLine(stream.toString());
                    text_output = stream.toString(); 
                    stream.close();
                }
            } while (false); 

            return ret; 
        }

        int uninit()
        {
            int ret = 0;
            return ret; 
        }

        language_type check_text_language( string text_input )
        {
            return language_type.LANGUAGE_CN; 
        }

        int nlp_service_loop()
        {
            int ret = 0;
            int _ret; 
            IPEndPoint ip_end_point;
            Socket listener;
            string text_output;
            //language_type _language_type; 

            do
            {
                ip_end_point = new IPEndPoint(IPAddress.Any, 8001);
                if (null == ip_end_point)
                {
                    ret = -1;
                    break;
                }

                try
                {
                    listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    if (null == listener)
                    {
                        ret = -1;
                        break;
                    }

                    listener.Bind(ip_end_point);
                    listener.Listen(20);//20 trucks 
                    // Start listening for connections. 
                    Console.WriteLine("nlp service started\n");

                    while (true)
                    {
                        Socket connection = listener.Accept();
                        Console.WriteLine("accepted client:{0}", connection.RemoteEndPoint.ToString());

                        do
                        {
                            string text_segmented = "";
                            //string text_pos_tagger = ""; 
                            //string text_ner = "";
                            string text_parsed = "";
                            //string text_su_time = ""; 
                            //string text_annotated = ""; 

                            string text_input = "";
                            byte[] data_input = new byte[16384];
                            byte[] data_output;
                            UInt32 data_size = 0;

                            _ret = data_io.receive_data(ref connection, ref data_input, ref data_size);
                            if (_ret != 0)
                            {
                                break;
                            }

                            if (data_size == 0)
                            {
                                break;
                            }

                            text_input += Encoding.UTF8.GetString(data_input, 0, (int)data_size);
                            Console.WriteLine(text_input);

                            //_language_type = check_text_language(text_input);

                            //if (_language_type != language_type.LANGUAGE_CN
                            //    && _language_type != language_type.LANGUAGE_EN)
                            //{
                            //    _ret = -1;
                            //    break;
                            //}

                            do
                            {
                                text_output = "";

                                ret = this.segment(text_input, ref text_segmented);
                                if (ret != 0)
                                {
                                    text_segmented = "";
                                    break;
                                }

                                text_input = text_segmented;

                                ret = this.parser(text_input, ref text_parsed);
                                if (ret != 0)
                                {
                                    text_parsed = "";
                                    break;
                                }
                                text_output += text_parsed;
                            } while (false); 

                            //text_input = text_segmented;

                            //ret = this.pos_tagger(text_input, ref text_pos_tagger);
                            //if (ret != 0)
                            //{
                            //    break;
                            //}

                            //if (_language_type == language_type.LANGUAGE_EN)
                            //{
                            //    text_input = text_segmented;

                            //    ret = this.ner(text_input, ref text_ner);
                            //    if (ret != 0)
                            //    {
                            //        break;
                            //    }

                            //    text_input = text_segmented;

                            //    ret = this.su_time(text_input, ref text_su_time);
                            //    if (ret != 0)
                            //    {
                            //        break;
                            //    }
                            //}

                            //text_input = text_segmented;

                            //ret = this.core_nlp(text_input, ref text_annotated);
                            //if (ret != 0)
                            //{
                            //    break;
                            //}

                            //now = DateTime.Now;

                            //text_output = "segment:\n";
                            //text_output += text_segmented;
                            //text_output += "\n";

                            //text_output += "pos tagger:\n";
                            //text_output += text_pos_tagger;
                            //text_output += "\n";

                            //text_output += "parsed:\n";
                           
                            text_output += "\n";
                            text_output += "\n";

                            //if (_language_type == language_type.LANGUAGE_EN)
                            //{
                            //    text_output += "ner:\n";
                            //    text_output += text_ner;
                            //    text_output += "\n";

                            //    text_output += "su time:\n";
                            //    text_output += text_su_time;
                            //    text_output += "\n";
                            //}

                            //text_output += "annotated:\n";
                            //text_output += text_annotated;
                            //text_output += "\n";

                            data_output = Encoding.UTF8.GetBytes(text_output.ToCharArray());
                            ret = data_io.send_data(ref connection, ref data_output, ref data_size);
                            if (ret != 0)
                            {
                                break;
                            }
                            Console.WriteLine(text_output);
                        } while (true) ;

                        Console.WriteLine("close client:{0}", connection.RemoteEndPoint.ToString());
                        connection.Close();
                    }
                }
                finally
                {
                    ret = -1;
                }

            } while (false);

            return ret; 
        }

        static void Main()
        {
            int ret = 0;
            nlp_service service;
            //string text_output; 

            do
            {
                service = new nlp_service();
                if (null == service)
                {
                    ret = -1;
                    break;
                }
                ret = service.init();
                if( ret != 0 )
                {
                    break; 
                }

                ret = service.nlp_service_loop();
                if (ret != 0)
                {
                    break;
                }

                //core_nlp();
                    //su_time();
                    //text_output = ""; 
                    //service.segment("通过以上操作并不能完全解决问题，因为显示出来的内容有可能不完全。可以先最小化，然后最大化命令行窗口，文件的内容就完整的显示出来了。", ref text_output);
                    //parser();
                    //ner();
                    //pos_tagger();
                } while (false); 
        }
    }
}