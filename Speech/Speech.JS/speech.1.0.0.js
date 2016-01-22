//----------------------------------------------------------------------
// Microsoft Speech SDK
// ====================
// 
// 
// FEATURES
// --------
// * Short-form recognition.
// * Long-form dictation.
// * Recognition with intent.
// * Integrated microphone support.
// * External audio support.
// 
// LICENSE
// -------
// © 2015 Microsoft. All rights reserved.  
// This document is provided “as-is”. Information and views expressed in this document, including URL and other Internet Web site references, may change without notice.  
// Some examples depicted herein are provided for illustration only and are fictitious.  No real association or connection is intended or should be inferred. 
// This document does not provide you with any legal rights to any intellectual property in any Microsoft product. You may copy and use this document for your internal, reference purposes. This 
// document is confidential and proprietary to Microsoft. It is disclosed and can be used only pursuant to a non-disclosure agreement. 
//----------------------------------------------------------------------

var Microsoft;
(function (Microsoft) {
    (function (ProjectOxford) {
        (function (SpeechRecognition) {
            (function (SpeechRecognitionMode) {
                SpeechRecognitionMode._map = [];
                SpeechRecognitionMode._map[0] = "shortPhrase";
                SpeechRecognitionMode.shortPhrase = 0;
                SpeechRecognitionMode._map[1] = "longDictation";
                SpeechRecognitionMode.longDictation = 1;
            })(SpeechRecognition.SpeechRecognitionMode || (SpeechRecognition.SpeechRecognitionMode = {}));
            var SpeechRecognitionMode = SpeechRecognition.SpeechRecognitionMode;
            var MicrophoneRecognitionClient = (function () {
                function MicrophoneRecognitionClient(prefs) {
                    this.onPartialResponseReceived = null;
                    this.onFinalResponseReceived = null;
                    this.onIntentReceived = null;
                    this.onError = null;
                    this._prefs = prefs;
                    this._sr = new Bing.Speech();
                }
                MicrophoneRecognitionClient.prototype.startMicAndRecognition = function () {
                    var _this = this;
                    this._sr.onresult = function (e) {
                        if (e.results[e.resultIndex].final) {
                            _this.onFinalResponseReceived(e.results[e.resultIndex]);
                        } else {
                            _this.onPartialResponseReceived(e.results[e.resultIndex][0].transcript);
                        }
                    };
                    this._sr.onerror = function (e) {
                        if (_this.onError) {
                            _this.onError(-1, JSON.stringify(e));
                        }
                    };
                    Bing.Platform.getCU().done(function (cu) {
                        cu.preferences = _this._prefs;
                        cu.onintent = function (intent) {
                            if (_this.onIntentReceived) {
                                _this.onIntentReceived(intent.payload);
                            }
                        };
                        _this._sr.start();
                    });
                };
                MicrophoneRecognitionClient.prototype.endMicAndRecognition = function () {
                    this._sr.stop();
                };
                return MicrophoneRecognitionClient;
            })();
            SpeechRecognition.MicrophoneRecognitionClient = MicrophoneRecognitionClient;            
            var DataRecognitionClient = (function () {
                function DataRecognitionClient(prefs) {
                    this.onPartialResponseReceived = null;
                    this.onFinalResponseReceived = null;
                    this.onIntentReceived = null;
                    this.onError = null;
                    this._prefs = prefs;
                    this._start = true;
                    this._sr = new Bing.Speech();
                }
                DataRecognitionClient.prototype.sendAudio = function (buffer, actualAudioBytesInBuffer) {
                    var _this = this;
                    var src = new Bing.ArrayBufferSource();
                    src.setBuffer(buffer);
                    this._sr.mediaSource = src;
                    this._sr.onresult = function (e) {
                        if (e.results[e.resultIndex].final) {
                            _this.onFinalResponseReceived(e.results[e.resultIndex]);
                        } else {
                            _this.onPartialResponseReceived(e.results[e.resultIndex][0].transcript);
                        }
                    };
                    this._sr.onerror = function (e) {
                        if (_this.onError) {
                            _this.onError(-1, JSON.stringify(e));
                        }
                    };
                    Bing.Platform.getCU().done(function (cu) {
                        cu.preferences = _this._prefs;
                        cu.onintent = function (intent) {
                            if (_this.onIntentReceived) {
                                _this.onIntentReceived(intent.payload);
                            }
                        };
                        _this._sr.start();
                    });
                };
                DataRecognitionClient.prototype.endAudio = function () {
                    this._start = true;
                };
                return DataRecognitionClient;
            })();
            SpeechRecognition.DataRecognitionClient = DataRecognitionClient;            
            (function (SpeechRecognitionServiceFactory) {
                function createDataClient(speechRecognitionMode, language, primaryKey, secondaryKey) {
                    return new SpeechRecognition.DataRecognitionClient(createPrefs(speechRecognitionMode, language, primaryKey, secondaryKey));
                }
                SpeechRecognitionServiceFactory.createDataClient = createDataClient;
                function createDataClientWithIntent(language, primaryKey, secondaryKey, luisAppId, luisSubscriptionId) {
                    var prefs = createPrefs(SpeechRecognition.SpeechRecognitionMode.shortPhrase, language, primaryKey, secondaryKey);
                    prefs.luisAppId = luisAppId;
                    prefs.luisSubscriptionId = luisSubscriptionId;
                    return new SpeechRecognition.DataRecognitionClient(prefs);
                }
                SpeechRecognitionServiceFactory.createDataClientWithIntent = createDataClientWithIntent;
                function createMicrophoneClient(speechRecognitionMode, language, primaryKey, secondaryKey) {
                    return new SpeechRecognition.MicrophoneRecognitionClient(createPrefs(speechRecognitionMode, language, primaryKey, secondaryKey));
                }
                SpeechRecognitionServiceFactory.createMicrophoneClient = createMicrophoneClient;
                function createMicrophoneClientWithIntent(language, primaryKey, secondaryKey, luisAppId, luisSubscriptionId) {
                    var prefs = createPrefs(SpeechRecognition.SpeechRecognitionMode.shortPhrase, language, primaryKey, secondaryKey);
                    prefs.luisAppId = luisAppId;
                    prefs.luisSubscriptionId = luisSubscriptionId;
                    return new SpeechRecognition.MicrophoneRecognitionClient(prefs);
                }
                SpeechRecognitionServiceFactory.createMicrophoneClientWithIntent = createMicrophoneClientWithIntent;
                SpeechRecognitionServiceFactory.BaseSpeechUrl = "https://websockets.platform.bing.com/ws/speech/recognize";
                function createPrefs(speechRecognitionMode, language, primaryKey, secondaryKey) {
                    var serviceUri = SpeechRecognitionServiceFactory.BaseSpeechUrl;
                    switch(speechRecognitionMode) {
                        case SpeechRecognition.SpeechRecognitionMode.longDictation:
                            serviceUri += "/continuous";
                            break;
                    }
                    return {
                        serviceUri: serviceUri,
                        locale: language,
                        clientId: primaryKey,
                        clientSecret: secondaryKey,
                        clientVersion: "4.0.150429",
                        authenticationScheme: "MAIS"
                    };
                }
                SpeechRecognitionServiceFactory.createPrefs = createPrefs;
            })(SpeechRecognition.SpeechRecognitionServiceFactory || (SpeechRecognition.SpeechRecognitionServiceFactory = {}));
            var SpeechRecognitionServiceFactory = SpeechRecognition.SpeechRecognitionServiceFactory;
        })(ProjectOxford.SpeechRecognition || (ProjectOxford.SpeechRecognition = {}));
        var SpeechRecognition = ProjectOxford.SpeechRecognition;
    })(Microsoft.ProjectOxford || (Microsoft.ProjectOxford = {}));
    var ProjectOxford = Microsoft.ProjectOxford;
})(Microsoft || (Microsoft = {}));
var Bing;
(function (Bing) {
    Bing._cu;
    Bing._cuDeferred = [];
    Bing._window = window;
    Bing._defaultVoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)";
    function write(text) {
        if (Bing._window.onlog) {
            Bing._window.onlog(text);
        }
        if (console && console.log) {
            console.log(text);
        }
    }
    Bing.write = write;
    function writeline(text) {
        write(text + "\n");
    }
    Bing.writeline = writeline;
    function setValue(name, value) {
        var json = JSON.stringify(value);
        window.localStorage.setItem(name, json);
    }
    Bing.setValue = setValue;
    function getValue(name) {
        var json = window.localStorage.getItem(name);
        var ret;
        if (json !== null && json != 'undefined') {
            ret = JSON.parse(json);
        }
        return ret;
    }
    Bing.getValue = getValue;
    function dispatchEvent(name) {
        var event = document.createEvent('Event');
        event.initEvent(name, true, true);
        window.dispatchEvent(event);
    }
    Bing.dispatchEvent = dispatchEvent;
    function dispatchAudioStart() {
        dispatchEvent('audiostart');
    }
    Bing.dispatchAudioStart = dispatchAudioStart;
    function dispatchAudioStop() {
        dispatchEvent('audiostop');
    }
    Bing.dispatchAudioStop = dispatchAudioStop;
    function useHttp() {
        return getValue("useHttp");
    }
    Bing.useHttp = useHttp;
    function devMode() {
        return getValue("devMode");
    }
    Bing.devMode = devMode;
    (function (SynthState) {
        SynthState._map = [];
        SynthState._map[0] = "None";
        SynthState.None = 0;
        SynthState._map[1] = "Pending";
        SynthState.Pending = 1;
        SynthState._map[2] = "Started";
        SynthState.Started = 2;
    })(Bing.SynthState || (Bing.SynthState = {}));
    var SynthState = Bing.SynthState;
    var Synthesis = (function () {
        function Synthesis() {
            var _this = this;
            this.voices = [];
            this._state = SynthState.None;
            this.paused = false;
            this.voices = null;
            var request = new XMLHttpRequest();
            request.open('GET', "https://speech.platform.bing.com/synthesize/list/voices", true);
            request.responseType = 'json';
            request.onload = function () {
                if (request.readyState == 4) {
                    _this.voices = [];
                    if (request.status === 200) {
                        var list = handleJSONWebResponse(request);
                        for(var i = 0; i < list.length; ++i) {
                            var vox = {
                                voiceURI: "https://speech.platform.bing.com/synthesize",
                                name: list[i].Name,
                                lang: list[i].Locale,
                                localService: false,
                                default: list[i].Name == Bing._defaultVoiceName
                            };
                            _this.voices.push(vox);
                            if (vox.default) {
                                _this.defaultVoice = vox;
                            }
                        }
                    }
                }
            };
            request.send();
        }
        Object.defineProperty(Synthesis.prototype, "pending", {
            get: function () {
                return this._state == SynthState.Pending;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(Synthesis.prototype, "speaking", {
            get: function () {
                return this._state == SynthState.Started;
            },
            enumerable: true,
            configurable: true
        });
        Synthesis.prototype.speak = function (utterance) {
            var _this = this;
            var start = Date.now();
            this._state = SynthState.Pending;
            Platform.getCU().done(function (cu) {
                cu.onttsstart = function () {
                    _this._state = SynthState.Started;
                    if (utterance.onstart) {
                        utterance.onstart(_this.createEvent("start", utterance));
                    }
                };
                cu.onttsend = function () {
                    _this._state = SynthState.None;
                    if (utterance.onend) {
                        var e = _this.createEvent("end", utterance);
                        e.elapsedTime = (Date.now() - start) / 1000;
                        utterance.onend(e);
                    }
                };
                cu.onevent = function () {
                    _this._state = SynthState.None;
                    utterance.onerror(_this.createEvent("error", utterance));
                };
                if (!_this.context) {
                    if (!AudioContext) {
                        throw "Sorry, your browser doesn't support WebAudio";
                    }
                    _this.context = new AudioContext();
                }
                cu.context = _this.context;
                cu.tts(utterance.text);
            });
        };
        Synthesis.prototype.cancel = function () {
            this._state = SynthState.None;
            Platform.getCU().done(function (cu) {
                cu.ttsStop();
            });
        };
        Synthesis.prototype.pause = function () {
        };
        Synthesis.prototype.resume = function () {
        };
        Synthesis.prototype.getVoices = function () {
            return this.voices;
        };
        Synthesis.prototype.createEvent = function (type, target) {
            var e = document.createEvent("UIEvent");
            e.initUIEvent(type, false, false, null, null);
            e.target = e.currentTarget = target;
            e.srcElement = target;
            e.timeStamp = Date.now();
            return e;
        };
        return Synthesis;
    })();
    Bing.Synthesis = Synthesis;    
    var msSpeechSynthesisUtterance = (function () {
        function msSpeechSynthesisUtterance(text) {
            this.text = text;
        }
        msSpeechSynthesisUtterance.prototype.removeEventListener = function (type, listener, useCapture) {
        };
        msSpeechSynthesisUtterance.prototype.addEventListener = function (type, listener, useCapture) {
        };
        msSpeechSynthesisUtterance.prototype.dispatchEvent = function (evt) {
            return false;
        };
        return msSpeechSynthesisUtterance;
    })();    
    var Speech = (function () {
        function Speech() {
            this._firstAudio = true;
            this.playbackAudioFilesOverride = false;
            this.grammars = null;
            this.lang = "en-US";
            this.continuous = false;
            this.interimResults = true;
            this.maxAlternatives = -1;
            this.serviceURI = null;
        }
        Speech.prototype.removeEventListener = function (type, listener, useCapture) {
        };
        Speech.prototype.addEventListener = function (type, listener, useCapture) {
        };
        Speech.prototype.dispatchEvent = function (evt) {
            return false;
        };
        Speech.prototype.start = function () {
            var _this = this;
            if (this.onstart) {
                this.onstart(this.createEvent("start"));
            }
            if (this.isMicSource) {
                if (!(navigator).getUserMedia) {
                    throw "Sorry, your browser doesn't have microphone support.";
                }
                (navigator).getUserMedia({
                    audio: true
                }, function (stream) {
                    if (!_this.context) {
                        if (!AudioContext) {
                            throw "Sorry, your browser doesn't support WebAudio";
                        }
                        _this.context = new AudioContext();
                    }
                    _this.currentSource = _this.context.createMediaStreamSource(stream);
                }, function () {
                    _this.HandleError();
                });
            } else {
                this.currentSource = this.mediaSource;
            }
        };
        Speech.prototype.stop = function () {
            if (this._currentSource) {
                Platform.getCU().done(function (cu) {
                    cu.disconnect();
                });
                if (this._currentSource) {
                    this._currentSource.disconnect();
                }
                this._currentSource = null;
            }
            if (this._currentDestination) {
                this._currentDestination.disconnect();
                this._currentDestination = null;
            }
        };
        Speech.prototype.abort = function () {
            this.stop();
        };
        Speech.prototype.HandleError = function () {
        };
        Object.defineProperty(Speech.prototype, "currentSource", {
            set: function (source) {
                var _this = this;
                this._firstAudio = true;
                this._currentSource = source;
                Platform.getCU().done(function (cu) {
                    cu.onend = function () {
                        if (_this.onend) {
                            _this.onend(_this.createEvent("end"));
                        }
                    };
                    cu.onaudioend = function () {
                        if (_this.onaudioend) {
                            _this.onaudioend(_this.createEvent("audioend"));
                        }
                    };
                    cu.ondisplaytext = function (text) {
                        if (_this.onresult) {
                            var e = _this.createEvent("result");
                            e.resultIndex = 0;
                            e.results = {
                                length: 1,
                                0: {
                                    final: false,
                                    length: 1,
                                    0: {
                                        transcript: text
                                    }
                                }
                            };
                            _this.onresult(e);
                        }
                    };
                    cu.onresult = function (result) {
                        if (result.status == 301) {
                            if (_this.onnomatch) {
                                _this.onnomatch(_this.createEvent("nomatch"));
                            }
                        } else if (_this.onresult) {
                            _this.onresult(result);
                        } else {
                            console.warn("Speech.onresult not set");
                        }
                    };
                    cu.onevent = function (statusCode) {
                        if (_this.onerror) {
                            var e = _this.createEvent("error");
                            e.error = "received a speech error " + statusCode;
                            _this.onerror(e);
                        }
                        if (_this.onend) {
                            _this.onend(_this.createEvent("end"));
                        }
                    };
                    _this._currentDestination = _this.createRecogitionDestination(_this._currentSource, cu, null);
                });
            },
            enumerable: true,
            configurable: true
        });
        Speech.prototype.createEvent = function (type) {
            var e = document.createEvent("UIEvent");
            e.initUIEvent(type, false, false, null, null);
            e.target = e.currentTarget = this;
            e.srcElement = this;
            e.timeStamp = Date.now();
            return e;
        };
        Speech.prototype.createRecogitionDestination = function (source, cu, onaudioprocess) {
            var _this = this;
            var destination = source.context.createScriptProcessor(4096, 1, 1);
            destination.onaudioprocess = function (e) {
                var inputBuffer = e.inputBuffer;
                if (_this._firstAudio) {
                    _this._firstAudio = false;
                    if (_this.onaudiostart) {
                        _this.onaudiostart(_this.createEvent("audiostart"));
                    }
                }
                if (Bing._window.useStringArrays === true) {
                    cu.audioprocess(new StringAudioBuffer(inputBuffer));
                } else {
                    cu.audioprocess(inputBuffer);
                }
                if (onaudioprocess) {
                    onaudioprocess(e);
                }
                if (_this.playbackAudioFiles() && !_this.isMicSource) {
                    var outputBuffer = e.outputBuffer;
                    for(var channel = 0; channel < outputBuffer.numberOfChannels; channel++) {
                        var outputData = outputBuffer.getChannelData(channel);
                        var inputData = inputBuffer.getChannelData(channel);
                        for(var sample = 0; sample < e.inputBuffer.length; sample++) {
                            outputData[sample] = inputData[sample];
                        }
                    }
                }
                if (Bing._window.msSpeechButton && !Bing._window.isActiveX) {
                    Bing._window.msSpeechButton.audioprocess(e);
                }
            };
            source.connect(destination);
            cu.connect(source);
            destination.connect(source.context.destination);
            return destination;
        };
        Object.defineProperty(Speech.prototype, "isMicSource", {
            get: function () {
                return null === this.mediaSource || typeof this.mediaSource == 'undefined';
            },
            enumerable: true,
            configurable: true
        });
        Speech.prototype.playbackAudioFiles = function () {
            return getValue("playbackAudioFiles") || this.playbackAudioFilesOverride;
        };
        return Speech;
    })();
    Bing.Speech = Speech;    
    ;
    var Task = (function () {
        function Task() {
            this.completed = false;
            this.startTime = Date.now();
        }
        Task.prototype.complete = function () {
            this.signalComplete(true);
        };
        Task.prototype.resolve = function (result) {
            this.signalComplete(result);
        };
        Task.prototype.done = function (cb) {
            if (this.completed) {
                cb(this.result);
                return;
            }
            this.cb = cb;
        };
        Object.defineProperty(Task.prototype, "elapsedTime", {
            get: function () {
                return (Date.now() - this.startTime) / 1000;
            },
            enumerable: true,
            configurable: true
        });
        Task.prototype.signalComplete = function (result) {
            this.result = result;
            this.completed = true;
            if (this.cb) {
                this.cb(this.result);
            }
        };
        return Task;
    })();
    Bing.Task = Task;    
    var StringAudioBuffer = (function () {
        function StringAudioBuffer(buffer) {
            this._audioBuffer = buffer;
            this.sampleRate = buffer.sampleRate;
            this.length = buffer.length;
            this.duration = buffer.duration;
            this.numberOfChannels = buffer.numberOfChannels;
        }
        StringAudioBuffer.prototype.getChannelData = function (channel) {
            var data = this._audioBuffer.getChannelData(channel);
            var a = [];
            var ret;
            var i;
            var byteStr;
            for(i = 0; i < data.length; ++i) {
                byteStr = Math.floor((data[i] + 1.0) * 0x7fff).toString(16).replace('-', '');
                while(byteStr.length < 4) {
                    byteStr = "0" + byteStr;
                }
                a.push(byteStr);
            }
            ret = a.join("");
            return ret;
        };
        StringAudioBuffer.prototype.copyFromChannel = function (destination, channelNumber, startInChannel) {
            this._audioBuffer.copyFromChannel(destination, channelNumber, startInChannel);
        };
        StringAudioBuffer.prototype.copyToChannel = function (source, channelNumber, startInChannel) {
            this._audioBuffer.copyToChannel(source, channelNumber, startInChannel);
        };
        return StringAudioBuffer;
    })();
    Bing.StringAudioBuffer = StringAudioBuffer;    
    function CreateActiveXObject(name) {
        try  {
            return new ActiveXObject(name);
        } catch (e) {
            return null;
        }
    }
    Bing.CreateActiveXObject = CreateActiveXObject;
    (function (Platform) {
        function isEdge() {
            return navigator.userAgent.indexOf("Edge/") != -1;
        }
        Platform.isEdge = isEdge;
        function isSafari() {
            return navigator.userAgent.indexOf("AppleWebKit") != -1;
        }
        Platform.isSafari = isSafari;
        function supportsPPAPI() {
            if (Bing._window.chrome && !isEdge() && navigator.userAgent.indexOf("Chrome/")) {
                return true;
            } else {
                return false;
            }
        }
        Platform.supportsPPAPI = supportsPPAPI;
        function supportsNPAPI() {
            if (!isEdge() && ((navigator.userAgent.indexOf("Firefox") != -1) || isSafari())) {
                return true;
            } else {
                return false;
            }
        }
        Platform.supportsNPAPI = supportsNPAPI;
        function supportsActiveX() {
            if ((navigator.userAgent.indexOf("Trident") != -1)) {
                return true;
            } else {
                return false;
            }
        }
        Platform.supportsActiveX = supportsActiveX;
        function getCU() {
            var task = new Bing.Task();
            if (Bing._cu) {
                task.resolve(Bing._cu);
            } else {
                Bing._cuDeferred.push(task);
            }
            return task;
        }
        Platform.getCU = getCU;
    })(Bing.Platform || (Bing.Platform = {}));
    var Platform = Bing.Platform;
    function initialize() {
        var i;
        Bing._window.useStringArrays = false;
        Bing._cu = createSpeech();
        if (!Bing._cuDeferred) {
            Bing._cuDeferred = [];
        }
        if (Bing._cu) {
            for(i = 0; i < Bing._cuDeferred.length; ++i) {
                Bing._cuDeferred[i].resolve(Bing._cu);
            }
        }
    }
    Bing.initialize = initialize;
    function decodeAudioData(context, audioData, successCallback, errorCallback) {
        if (Bing._window.isActiveX) {
            audioData = new Int8Array(audioData);
        }
        context.decodeAudioData(audioData, successCallback, errorCallback);
    }
    Bing.decodeAudioData = decodeAudioData;
    function handleJSONWebResponse(xhr) {
        if (typeof (xhr.response) == "string") {
            return JSON.parse(xhr.response);
        }
        return xhr.response;
    }
    Bing.handleJSONWebResponse = handleJSONWebResponse;
    var OxfordAuthenticator = (function () {
        function OxfordAuthenticator() {
        }
        OxfordAuthenticator.prototype.authenticate = function (primaryKey, secondaryKey) {
            var _this = this;
            var task = new Task();
            var now = Date.now();
            var params = [
                "grant_type=client_credentials&client_id=", 
                encodeURIComponent(primaryKey), 
                "&client_secret=", 
                encodeURIComponent(secondaryKey), 
                "&scope=", 
                encodeURIComponent("https://speech.platform.bing.com")
            ].join("");
            if (!this._response || !this._expireTime || Date.now() >= this._expireTime.getTime()) {
                writeline("refreshing token");
                var xhr = new XMLHttpRequest();
                xhr.open('POST', "https://oxford-speech.cloudapp.net/token/issueToken", true);
                xhr.responseType = 'json';
                xhr.onload = function () {
                    if (xhr.readyState == 4) {
                        if (xhr.status === 200) {
                            _this._response = handleJSONWebResponse(xhr);
                            _this._expireTime = new Date(Date.now() + parseInt(_this._response.expires_in) * 1000);
                            task.resolve("Bearer " + _this._response.access_token);
                        } else {
                            task.resolve(null);
                        }
                    }
                };
                xhr.onerror = function () {
                    task.resolve(null);
                };
                xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                xhr.send(params);
            } else {
                task.resolve("Bearer " + this._response.access_token);
            }
            return task;
        };
        return OxfordAuthenticator;
    })();    
    var AdmAuthenticator = (function () {
        function AdmAuthenticator() {
        }
        AdmAuthenticator.prototype.authenticate = function (primaryKey, secondaryKey) {
            var task = new Task();
            writeline("authenticate: " + primaryKey + " " + secondaryKey);
            var params = "grant_type=client_credentials&client_id=" + encodeURIComponent(primaryKey) + "&client_secret=" + encodeURIComponent(secondaryKey) + "&scope=" + encodeURIComponent("https://speech.platform.bing.com");
            var xhr = new XMLHttpRequest();
            xhr.open('POST', "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13", true);
            xhr.onload = function () {
                if (xhr.readyState == 4) {
                    if (xhr.status === 200) {
                        task.resolve(handleJSONWebResponse(xhr));
                    } else {
                        task.resolve(null);
                    }
                }
            };
            xhr.onerror = function () {
                task.resolve(null);
            };
            xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
            xhr.send(params);
            return task;
        };
        return AdmAuthenticator;
    })();    
    var Riff = (function () {
        function Riff(sampleRate, bitsPerSample) {
            this._buffer = [];
            this._bitsPerSample = 8;
            this._channels = 1;
            this._sampleRate = 44100;
            this._bitsPerSample = bitsPerSample;
            this._sampleRate = sampleRate;
            this.appendString("RIFF");
            this.appendUINT32(0);
            this.appendString("WAVEfmt ");
            this.appendUINT32(2 + 2 + 4 + 4 + 2 + 2);
            this.appendUINT16(Riff.WAVE_FORMAT_PCM);
            this.appendUINT16(this._channels);
            this.appendUINT32(this._sampleRate);
            this.appendUINT32(this._sampleRate * (this._bitsPerSample >> 3) * this._channels);
            this.appendUINT16(this._bitsPerSample >> 3);
            this.appendUINT16(this._bitsPerSample);
            this.appendString("data");
            this.appendUINT32(0);
        }
        Riff.WAVE_FORMAT_PCM = 1;
        Riff.WAVE_FORMAT_IEEE_FLOAT = 3;
        Riff.prototype.appendString = function (s) {
            for(var i = 0; i < s.length; ++i) {
                this._buffer.push(s.charCodeAt(i));
            }
        };
        Riff.prototype.appendUINT32 = function (n) {
            this.appendUINT16(n);
            this.appendUINT16(n >> 16);
        };
        Riff.prototype.appendUINT16 = function (n) {
            this._buffer.push((n & 0x00ff) >> 0);
            this._buffer.push((n & 0xff00) >> 8);
        };
        Riff.prototype.toByteArray = function () {
            return this._buffer;
        };
        return Riff;
    })();    
    var LuisClient = (function () {
        function LuisClient(prefs) {
            this._prefs = prefs;
            switch(prefs.authenticationScheme) {
                case "MAIS":
                    this._auth = new OxfordAuthenticator();
                    break;
                case "ADM":
                    this._auth = new AdmAuthenticator();
                    break;
            }
        }
        LuisClient.kServiceUrl = "https://api.projectoxford.ai/luis/v1/application?subscription-key=";
        LuisClient.prototype.getIntent = function (text) {
            var _this = this;
            var task = new Task();
            var request = new XMLHttpRequest();
            this._auth.authenticate(this._prefs.clientId, this._prefs.clientSecret).done(function (token) {
                if (!token) {
                    task.resolve(null);
                    return;
                }
                request.open('GET', [
                    LuisClient.kServiceUrl, 
                    _this._prefs.luisSubscriptionId, 
                    "&id=", 
                    _this._prefs.luisAppId, 
                    "&q=", 
                    text
                ].join(""), true);
                request.setRequestHeader("Authorization", token);
                request.onload = function () {
                    if (request.readyState == 4 && request.status === 200) {
                        var response = handleJSONWebResponse(request);
                        task.resolve(request.response);
                    } else {
                        task.resolve(null);
                    }
                };
                request.send();
            });
            return task;
        };
        return LuisClient;
    })();
    Bing.LuisClient = LuisClient;    
    var HTTPResultStatus = (function () {
        function HTTPResultStatus() { }
        HTTPResultStatus.SUCCESS = "success";
        HTTPResultStatus.ERROR = "error";
        return HTTPResultStatus;
    })();    
    var HttpClient = (function () {
        function HttpClient() {
            this.queue = [];
            this.responseFormat = "json";
            writeline("Defaulting to http client");
            this.requestUri = "?scenarios=smd";
            this.requestUri += "&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5";
            this.requestUri += "&device.os=wp7";
            this.requestUri += "&version=3.0";
            this.requestUri += "&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3";
            this.requestUri += "&requestid=" + "2065F912-3699-408C-A80A-9D17F42B9692";
        }
        HttpClient.prototype.removeEventListener = function (type, listener, useCapture) {
        };
        HttpClient.prototype.addEventListener = function (type, listener, useCapture) {
            writeline("addEventListener: " + type + " " + this);
        };
        HttpClient.prototype.dispatchEvent = function (evt) {
            return false;
        };
        HttpClient.prototype.connect = function (destination, output, input) {
            Bing._window.useStringArrays = false;
            this.sourceSampleRate = destination.context.sampleRate;
            this.sampleRate = 16000;
            if (this.sourceSampleRate <= 0) {
                return;
            }
            this.connected = true;
            if (this.preferences.luisAppId && this.preferences.luisSubscriptionId) {
                this.luis = new LuisClient(this.preferences);
            }
            this.queue = [];
            this.buffer = new Int8Array(new Riff(this.sampleRate, 8).toByteArray());
            this.offset = this.buffer.length;
        };
        HttpClient.prototype.disconnect = function () {
            if (this.connected) {
                this.connected = false;
                if (this.onaudioend) {
                    this.onaudioend();
                }
                this.send();
            }
        };
        Object.defineProperty(HttpClient.prototype, "preferences", {
            get: function () {
                return this._preferences;
            },
            set: function (prefs) {
                this._preferences = prefs;
                this.auth = null;
            },
            enumerable: true,
            configurable: true
        });
        HttpClient.prototype.sendText = function (inputText) {
        };
        HttpClient.prototype.audioprocess = function (buffer) {
            this.appendAsUInt8(buffer.getChannelData(0));
        };
        HttpClient.prototype.tts = function (text, contentType, outputFormat) {
            var _this = this;
            if (!contentType) {
                contentType = "text/plain";
            }
            if (!outputFormat) {
                outputFormat = "riff-16khz-16bit-mono-pcm";
            }
            if (contentType === "text/plain") {
                text = "<?xml version='1.0' encoding='UTF-8'?>" + "<speak version='1.0' xml:lang='" + this.preferences.locale + "'>" + "<voice xml:lang='" + this.preferences.locale + "' name='" + Bing._defaultVoiceName + "'>" + text + "</voice></speak>";
                contentType = "application/ssml+xml";
            }
            var request = new XMLHttpRequest();
            request.open('POST', "https://speech.platform.bing.com/synthesize", true);
            request.responseType = 'arraybuffer';
            request.setRequestHeader("X-MICROSOFT-OutputFormat", outputFormat);
            request.setRequestHeader("Content-Type", contentType);
            request.onload = function () {
                if (request.readyState == 4 && request.status !== 200) {
                    _this.onevent(request.status);
                } else {
                    _this.renderAudio(_this.context, request.response);
                }
            };
            this.getToken().done(function (token) {
                if (!token) {
                    _this.dispatchError(-1);
                    return;
                }
                request.setRequestHeader("Authorization", token);
                if (_this.onttsstart) {
                    _this.onttsstart();
                }
                request.send(text);
            });
        };
        HttpClient.prototype.ttsStop = function () {
            var src = this.ttsSource;
            if (src && this.context.state != "suspended") {
                try  {
                    src.stop();
                } catch (e) {
                    writeline("ttsStop: buffer source failed to stop. state: " + this.context.state + " exception:" + e);
                }
            }
        };
        HttpClient.prototype.getToken = function () {
            if (!this.auth) {
                switch(this.preferences.authenticationScheme) {
                    case "MAIS":
                        this.auth = new OxfordAuthenticator();
                        break;
                    case "ADM":
                        this.auth = new AdmAuthenticator();
                        break;
                }
            }
            return this.auth.authenticate(this.preferences.clientId, this.preferences.clientSecret);
        };
        HttpClient.prototype.send = function () {
            var _this = this;
            var result;
            this.getToken().done(function (token) {
                if (!token) {
                    _this.dispatchError(-1);
                    return;
                }
                var serviceUrl = _this.preferences.serviceUri.replace("/ws/speech", "").replace("websockets.", "speech.");
                writeline("connect: url " + serviceUrl);
                var request = new XMLHttpRequest();
                request.open('POST', [
                    serviceUrl, 
                    _this.requestUri, 
                    "&locale=", 
                    _this.preferences.locale, 
                    "&format=", 
                    _this.responseFormat
                ].join(""), true);
                request.responseType = _this.responseFormat;
                request.setRequestHeader("Content-Type", 'audio/wav; codec="audio/pcm"; samplerate=' + _this.sampleRate);
                request.setRequestHeader("Authorization", token);
                token = token;
                request.onload = function () {
                    if (request.readyState == 4 && request.status !== 200) {
                        _this.dispatchError(request.status);
                    } else {
                        result = handleJSONWebResponse(request);
                        if (result.header.status === HTTPResultStatus.ERROR) {
                            _this.dispatchError(-1);
                        } else {
                            _this.dispatchResult(result);
                        }
                    }
                };
                if (_this.buffer && _this.buffer.length) {
                    request.send(_this.buffer);
                }
            });
        };
        HttpClient.prototype.dispatchError = function (statusCode) {
            if (this.onevent) {
                this.onevent(statusCode);
            }
        };
        HttpClient.prototype.dispatchResult = function (result) {
            var _this = this;
            var reco;
            if (result.results && result.results.length > 0 && result.results[0].name) {
                reco = result.results[0].name;
            }
            if (this.luis && this.onintent) {
                this.luis.getIntent(reco).done(function (r) {
                    _this.onintent({
                        payload: r
                    });
                });
            }
            if (this.onresult) {
                var phrases = [];
                for(var i = 0; i < result.results.length; ++i) {
                    var r = result.results[i];
                    phrases.push({
                        lexical: r.lexical,
                        display: r.name,
                        inverseNormalization: null,
                        maskedInverseNormalization: null,
                        transcript: r.name,
                        confidence: parseFloat(r.confidence)
                    });
                }
                phrases["final"] = true;
                var finalResult = {
                    resultIndex: 0,
                    results: {
                        length: 1,
                        0: phrases
                    },
                    interpretation: reco,
                    emma: null,
                    status: 200
                };
                this.onresult(finalResult);
            }
            if (this.onend) {
                this.onend();
            }
        };
        HttpClient.prototype.appendAsUInt8 = function (a) {
            var newBuffer;
            newBuffer = new Int8Array(a.length + this.offset);
            if (this.buffer) {
                newBuffer.set(this.buffer, 0);
            }
            this.buffer = newBuffer;
            var incrementBy = this.sourceSampleRate / this.sampleRate;
            for(var i = 0; i < a.length; i += incrementBy) {
                this.buffer[this.offset++] = Math.floor((a[Math.floor(i)] - .5) * 128);
            }
        };
        HttpClient.prototype.renderAudio = function (context, audioData) {
            var _this = this;
            decodeAudioData(context, audioData, function (buffer) {
                writeline("completed decoding audio");
                _this.ttsSource = context.createBufferSource();
                _this.ttsSource.buffer = buffer;
                _this.ttsSource.connect(_this.context.destination);
                if (_this.onttsstart) {
                    _this.onttsstart();
                }
                _this.ttsSource.start(0);
                _this.ttsSource.onended = function () {
                    if (_this.onttsend) {
                        _this.onttsend();
                    }
                    _this.ttsSource = null;
                };
            }, function () {
                writeline("error decoding audio");
                _this.onevent(-1);
            });
        };
        return HttpClient;
    })();
    Bing.HttpClient = HttpClient;    
    var NaclClient = (function () {
        function NaclClient() {
            var _this = this;
            var chrome = Bing._window.chrome;
            var naclContainer;
            if (devMode() === true) {
                naclContainer = document.createElement("div");
                naclContainer.setAttribute("style", "width:0;height:0");
                naclContainer.addEventListener('load', function (arg) {
                    var i;
                    writeline("plugin load:" + arg);
                    for(i = 0; i < Bing._cuDeferred.length; ++i) {
                        Bing._cuDeferred[i].resolve(Bing._cu);
                    }
                }, true);
                naclContainer.addEventListener('error', function (arg) {
                    writeline("plugin error:" + arg);
                }, true);
                naclContainer.addEventListener('crash', function (arg) {
                    writeline("plugin crash:" + arg);
                }, true);
                naclContainer.addEventListener('message', function (arg) {
                    _this.handleMessage(arg);
                }, true);
                window.document.body.appendChild(naclContainer);
                var npPlugin = document.createElement("embed");
                npPlugin.setAttribute("type", "application/x-pnacl");
                npPlugin.setAttribute("src", "/bin/pepper_speech.nmf");
                npPlugin.setAttribute("id", "pepper_speech");
                this._module = npPlugin;
                naclContainer.appendChild(npPlugin);
            } else {
                var port = chrome.runtime.connect(NaclClient.kKeyId);
                port.onMessage.addListener(function (arg) {
                    _this.handleMessage(arg);
                });
                port.onConnect = function (arg) {
                    writeline("port onConnect:" + arg);
                };
                this._module = port;
            }
        }
        NaclClient.kKeyId = "jffoigoenpgbgnhpchggjapfijhffghe";
        NaclClient.prototype.postMessage = function (arg) {
            if (Bing._window.naclNotInstalled === true) {
                return;
            }
            try  {
                this._module.postMessage(arg);
            } catch (e) {
                if (Bing._window.chrome.runtime.lastError) {
                    Bing._window.naclNotInstalled = true;
                    Bing.initialize();
                }
            }
        };
        NaclClient.prototype.handleMessage = function (arg) {
            var data;
            if ((arg).name) {
                data = arg;
            } else {
                data = arg.data;
            }
            if (this[data.name]) {
                this[data.name](data.data);
            }
        };
        NaclClient.prototype.log = function (arg) {
            var d = new Date();
            Bing.write("[" + d.toISOString() + "] " + arg);
        };
        NaclClient.prototype.removeEventListener = function (type, listener, useCapture) {
        };
        NaclClient.prototype.addEventListener = function (type, listener, useCapture) {
            writeline("addEventListener: " + type + " " + this);
        };
        NaclClient.prototype.dispatchEvent = function (evt) {
            return false;
        };
        NaclClient.prototype.connect = function (destination, output, input) {
            this.postMessage([
                "connect", 
                destination.context.sampleRate
            ]);
        };
        NaclClient.prototype.disconnect = function () {
            this.postMessage([
                "disconnect"
            ]);
        };
        Object.defineProperty(NaclClient.prototype, "preferences", {
            get: function () {
                return this._preferences;
            },
            set: function (prefs) {
                this._preferences = prefs;
                this.postMessage([
                    "setPreferences", 
                    prefs
                ]);
            },
            enumerable: true,
            configurable: true
        });
        NaclClient.prototype.sendText = function (inputText) {
        };
        NaclClient.prototype.audioprocess = function (buffer) {
            this.postMessage([
                "audioprocess", 
                buffer.getChannelData(0), 
                buffer
            ]);
        };
        NaclClient.prototype.tts = function (text, contentType, outputFormat) {
            this.postMessage([
                "tts", 
                text, 
                contentType, 
                outputFormat
            ]);
            if (this.onttsstart) {
                this.onttsstart();
            }
        };
        NaclClient.prototype.ttsStop = function () {
            this.postMessage([
                "ttsstop"
            ]);
        };
        return NaclClient;
    })();
    Bing.NaclClient = NaclClient;    
    function shouldCreateHttp() {
        if (Platform.isEdge()) {
            return true;
        }
        if (((Bing)).checkedForPluginInstall && ((Bing)).checkedForPluginInstall() !== true) {
            return false;
        }
        return true;
    }
    Bing.shouldCreateHttp = shouldCreateHttp;
    function createSpeech() {
        var cu;
        if (useHttp()) {
            return new HttpClient();
        } else if (Bing._window.naclNotInstalled !== true) {
            if (Platform.supportsPPAPI()) {
                Bing._window.chrome.runtime.sendMessage(NaclClient.kKeyId, "__hello", null, function (response) {
                    Bing._window.naclNotInstalled = (response !== "__hello");
                    if (Bing._window.naclNotInstalled) {
                        Bing._cu = new HttpClient();
                    } else {
                        Bing._cu = new NaclClient();
                    }
                    for(var i = 0; i < Bing._cuDeferred.length; ++i) {
                        Bing._cuDeferred[i].resolve(Bing._cu);
                    }
                });
                return null;
            } else if (Platform.supportsNPAPI()) {
                var npPlugin = document.createElement("embed");
                var promise = new Task();
                (npPlugin).type = "application/x-bingspeech";
                (npPlugin).data = "data:application/x-bingspeech,";
                npPlugin.setAttribute("style", "width:0;height:0");
                promise.done(function () {
                    window.document.body.appendChild(npPlugin);
                    try  {
                        (npPlugin)();
                        Bing._cu = npPlugin;
                        (Bing._cu).origin = window.window.location.href;
                        Bing._window.useStringArrays = true;
                    } catch (e) {
                        Bing._cu = new HttpClient();
                    }
                    for(var i = 0; i < Bing._cuDeferred.length; ++i) {
                        Bing._cuDeferred[i].resolve(Bing._cu);
                    }
                });
                if (window.document.body) {
                    promise.complete();
                } else {
                    document.addEventListener('DOMContentLoaded', function () {
                        promise.complete();
                    });
                }
                return null;
            } else if (Platform.supportsActiveX()) {
                cu = CreateActiveXObject("Bing.Host");
                if (cu) {
                    (cu).origin = window.window.location.href;
                    Bing._window.isActiveX = true;
                }
            }
        }
        if (!cu && shouldCreateHttp()) {
            writeline("Defaulting to http client");
            return new HttpClient();
        }
        if (!cu && ((Bing)).SpeechInstaller) {
            var installer = new ((Bing)).SpeechInstaller();
            installer.show("Install the Bing Speech Extender");
            return installer;
        }
        return cu;
    }
    Bing.createSpeech = createSpeech;
    var WebAudioSource = (function () {
        function WebAudioSource(url, context) {
            this.numberOfInputs = 1;
            this.numberOfOutputs = 1;
            this.channelCount = 1;
            this._url = url;
            if (!context) {
                if (!AudioContext) {
                    throw "Sorry, your browser doesn't support WebAudio";
                }
                this.context = new AudioContext();
            } else {
                this.context = context;
            }
            writeline("LogSendAudio: url=[" + url + "], contentType=[audio/basic]");
        }
        WebAudioSource.prototype.removeEventListener = function (type, listener, useCapture) {
        };
        WebAudioSource.prototype.addEventListener = function (type, listener, useCapture) {
        };
        WebAudioSource.prototype.dispatchEvent = function (evt) {
            return false;
        };
        WebAudioSource.prototype.connect = function (destination, output, input) {
            var _this = this;
            var request;
            this._destination = destination;
            if (null != this._aBuffer && this._aBuffer.byteLength > 0) {
                this.bufferReceived();
                return;
            }
            writeline("connect " + this);
            request = new XMLHttpRequest();
            request.open('GET', this._url, true);
            request.responseType = 'arraybuffer';
            request.onload = function () {
                if (request.readyState == 4 && request.status !== 200) {
                    _this.handleEnd(request.status);
                } else {
                    _this._aBuffer = request.response;
                    _this.bufferReceived();
                }
            };
            request.send();
        };
        WebAudioSource.prototype.disconnect = function () {
            var dest = this._destination;
            if (null == dest) {
                return;
            }
            if (this._bufferSource) {
                this._bufferSource.disconnect();
                this._bufferSource.stop();
            }
            this._started = false;
            dest.disconnect();
            this._destination = null;
            Platform.getCU().done(function (cu) {
                cu.disconnect();
            });
            if (this.onended) {
                this.onended();
            }
            dispatchAudioStop();
        };
        WebAudioSource.prototype.start = function (when, offset, duration) {
        };
        WebAudioSource.prototype.stop = function (when) {
        };
        WebAudioSource.prototype.setBuffer = function (buffer) {
            this._aBuffer = buffer;
            this.bufferReceived();
        };
        WebAudioSource.prototype.bufferReceived = function () {
            var _this = this;
            var bufferSource;
            bufferSource = this.context.createBufferSource();
            decodeAudioData(this.context, this._aBuffer, function (buffer) {
                _this._bufferSource = bufferSource;
                _this._bufferSource.buffer = buffer;
                _this.onBufferLoaded();
            }, function () {
                writeline("error decoding WebAudio");
                _this.handleEnd();
            });
        };
        WebAudioSource.prototype.handleEnd = function (err) {
            writeline("Source ended: err='" + err + "' " + this);
            this.disconnect();
        };
        WebAudioSource.prototype.onBufferLoaded = function () {
            var _this = this;
            this._bufferSource.connect(this._destination);
            this._bufferSource.start(0);
            this._started = true;
            this._bufferSource.onended = function () {
                _this.handleEnd();
            };
        };
        WebAudioSource.prototype.toString = function () {
            return this._url;
        };
        return WebAudioSource;
    })();
    Bing.WebAudioSource = WebAudioSource;    
    var ArrayBufferSource = (function () {
        function ArrayBufferSource(context) {
            this.numberOfInputs = 1;
            this.numberOfOutputs = 1;
            this.channelCount = 1;
            if (!context) {
                if (!AudioContext) {
                    throw "Sorry, your browser doesn't support WebAudio";
                }
                this.context = new AudioContext();
            } else {
                this.context = context;
            }
        }
        ArrayBufferSource.prototype.removeEventListener = function (type, listener, useCapture) {
        };
        ArrayBufferSource.prototype.addEventListener = function (type, listener, useCapture) {
        };
        ArrayBufferSource.prototype.dispatchEvent = function (evt) {
            return false;
        };
        ArrayBufferSource.prototype.connect = function (destination, output, input) {
            this._destination = destination;
            if (null != this._aBuffer && this._aBuffer.byteLength > 0) {
                this.bufferReceived();
                return;
            }
            writeline("connect " + this);
        };
        ArrayBufferSource.prototype.disconnect = function () {
            var dest = this._destination;
            if (null == dest) {
                return;
            }
            if (this._bufferSource) {
                this._bufferSource.disconnect();
                this._bufferSource.stop();
            }
            this._started = false;
            dest.disconnect();
            this._destination = null;
            Platform.getCU().done(function (cu) {
                cu.disconnect();
            });
            if (this.onended) {
                this.onended();
            }
            dispatchAudioStop();
        };
        ArrayBufferSource.prototype.start = function (when, offset, duration) {
        };
        ArrayBufferSource.prototype.stop = function (when) {
        };
        ArrayBufferSource.prototype.setBuffer = function (buffer) {
            this._aBuffer = buffer;
        };
        ArrayBufferSource.prototype.bufferReceived = function () {
            var _this = this;
            var bufferSource;
            bufferSource = this.context.createBufferSource();
            decodeAudioData(this.context, this._aBuffer, function (buffer) {
                _this._bufferSource = bufferSource;
                _this._bufferSource.buffer = buffer;
                _this.onBufferLoaded();
            }, function () {
                writeline("error decoding WebAudio");
                _this.handleEnd();
            });
        };
        ArrayBufferSource.prototype.handleEnd = function (err) {
            writeline("Source ended: err='" + err + "' " + this);
            this.disconnect();
        };
        ArrayBufferSource.prototype.onBufferLoaded = function () {
            var _this = this;
            this._bufferSource.connect(this._destination);
            this._bufferSource.start(0);
            this._started = true;
            this._bufferSource.onended = function () {
                _this.handleEnd();
            };
        };
        return ArrayBufferSource;
    })();
    Bing.ArrayBufferSource = ArrayBufferSource;    
    function SpeechMain() {
        var mediaNav = navigator;
        var acWindow = window;
        mediaNav.getUserMedia = mediaNav.getUserMedia || mediaNav.mozGetUserMedia || mediaNav.webkitGetUserMedia || mediaNav.msGetUserMedia || CreateActiveXObject("Bing.GetUserMedia");
        acWindow.AudioContext = acWindow.AudioContext || acWindow.webkitAudioContext || CreateActiveXObject("Bing.AudioContext");
        Bing._window.SpeechSynthesisUtterance = Bing._window.SpeechSynthesisUtterance || msSpeechSynthesisUtterance;
        Bing._window.SpeechRecognition = Bing.Speech;
        Bing._window.msSpeechSynthesis = new Bing.Synthesis();
        Bing._window.speechSynthesis = Bing._window.msSpeechSynthesis;
        Bing.initialize();
    }
    SpeechMain();
})(Bing || (Bing = {}));
