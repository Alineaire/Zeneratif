(
var warray;
var nodes;
var durs;

var bps = 175.0 / 60.0;  // Beats per second

s.boot;
NetAddr.langPort;
s.freeAll;

SynthDef(\mysynth, {
	//Piano synth based on Mda Plugin
	|out = 0, freq= 432, gate= 1, vel= 80, attack=0, decay= 0.8, release= 0.8, hard= 0.4, velhard= 0.8, muffle= 0.8, velmuff= 0.8, velcurve= 0.8, stereo= 0.2, tune= 0.5, random= 0.1, stretch= 0.1, sustain= 0, mul= 1, add= 0, amp = 0.5|
	var snd;
	snd = MdaPiano.ar(freq, gate, vel, decay, release, hard, velhard, muffle, velmuff, velcurve, stereo, tune, random, stretch, sustain, mul, add);
	//snd = snd * EnvGen.kr(Env.asr(attack, 1, 0.1), gate, doneAction: Done.freeSelf);
	snd = snd * EnvGen.kr(Env(times: [attack, Rand(1, 10)]), doneAction: Done.freeSelf);
	Out.ar(out, snd * amp);
}).add;

SynthDef(\kalimba, {
	//Kalimba based on bank of ressonators
	|out = 0, freq = 432, amp = 0.1, mix = 0.1, relMin = 2.5, relMax = 3.5|
	var snd;
	// Basic tone is a SinOsc
	snd = SinOsc.ar(freq) * EnvGen.ar(Env.perc(0.005, Rand(relMin, relMax), 1, -8), doneAction: 2);
	// The "clicking" sounds are modeled with a bank of resonators excited by enveloped pink noise
	snd = (snd * (1 - mix)) + (DynKlank.ar(`[
		// the resonant frequencies are randomized a little to add variation
		// there are two high resonant freqs and one quiet "bass" freq to give it some depth
		[240*ExpRand(0.9, 1.1), 2020*ExpRand(0.9, 1.1), 3151*ExpRand(0.9, 1.1)],
		[-7, 0, 3].dbamp,
		[0.8, 0.05, 0.07]
	], PinkNoise.ar * EnvGen.ar(Env.perc(0.001, 0.01))) * mix);
	Out.ar(out, Pan2.ar(snd, 0, amp));
	//By Nathan Ho aka Snappiz
	//http://sccode.org/1-51l
}).add;


SynthDef(\violin, {
	|  freq = 400,  amp=0.8 |
	var env = EnvGen.kr(Env(times: [0.1, Rand(1.0, 6.0)]), doneAction: 2);
	var sig = VarSaw.ar(
		freq,
		width:LFNoise2.kr(1).range(0.2, 0.8)*SinOsc.kr(5, Rand(0.0, 1.0)).range(0.7,0.8))*0.25;
	sig = sig * env * amp;
	Out.ar(0, sig!2);
	//It is also used for simulating Farfisa organ and flutes
	//by nicolaariutti
	//http://sccode.org/1-5as
}).add;

warray = {
	|collection, weightIndex = 0, valueIndex = 1|
	var weights = Array.fill(collection.size, {|i| collection[i][weightIndex]}).normalizeSum;
	Routine {
		inf.do {
			collection.wchoose(weights)[valueIndex].yield;
		};
	}
};

// [[notes], warray.value([[proba, next_node], ...])]
nodes = [


	[\kalimba, -25, warray.value([
		[1, [
			[warray.value([[1, [84]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [91]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [93]]]), warray.value([[20, 4], [80, 5]])],
			[warray.value([[1, [48]]]), warray.value([[50, 6], [50, 6]])],
			[warray.value([[1, [48, 79, 86, 60]]]), warray.value([[50, 6], [50, 7]])],
			[warray.value([[1, [72, 50, 79]]]), warray.value([[73, 7], [27, 1] ])],
			[warray.value([[1, [74]]]), warray.value([[50, 0], [50, 7]])],
			[warray.value([[1, [69]]]), warray.value([[100, -1]])],
		]],
		[1, [
			[warray.value([[1, [60]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [67]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [69]]]), warray.value([[20, 4], [80, 5]])],
			[warray.value([[1, [62]]]), warray.value([[50, 6], [50, 6]])],
			[warray.value([[1, [72, 79, 86, 60]]]), warray.value([[50, 6], [50, 7]])],
			[warray.value([[1, [72, 50, 79]]]), warray.value([[73, 7], [27, 1] ])],
			[warray.value([[1, [74]]]), warray.value([[50, 0], [50, 7]])],
			[warray.value([[1, [69]]]), warray.value([[100, -1]])],
		]],
	])],

	[\mysynth, -3, warray.value([
		[3, [
			[warray.value([[1, [36]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [33]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [69]]]), warray.value([[20, 4], [80, 5]])],
			[warray.value([[1, [62]]]), warray.value([[50, 6], [50, 6]])],
			[warray.value([[1, [72, 79, 86, 60]]]), warray.value([[50, 6], [50, 7]])],
			[warray.value([[1, [72, 50, 79]]]), warray.value([[73, 7], [27, 1] ])],
			[warray.value([[1, [74]]]), warray.value([[50, 0], [50, 7]])],
			[warray.value([[1, [69]]]), warray.value([[100, -1]])],
		]],
		[1, [
			[warray.value([[1, [60]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [67]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [69]]]), warray.value([[20, 4], [80, 5]])],
			[warray.value([[1, [62]]]), warray.value([[50, 6], [50, 6]])],
			[warray.value([[1, [72, 79, 86, 60]]]), warray.value([[50, 6], [50, 7]])],
			[warray.value([[1, [72, 50, 79]]]), warray.value([[73, 7], [27, 1] ])],
			[warray.value([[1, [74]]]), warray.value([[50, 0], [50, 7]])],
			[warray.value([[1, [69]]]), warray.value([[100, -1]])],
		]],
	])],

	[\violin, -20, warray.value([
		[1, [
			[warray.value([[1, [84-48]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [91-48]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [93-48]]]), warray.value([[20, 4], [80, 5]])],
			[warray.value([[1, [48-12]]]), warray.value([[50, 6], [50, 6]])],
			[warray.value([[1, [48, 79-36, 86-48, 60]]]), warray.value([[50, 6], [50, 7]])],
			[warray.value([[1, [72-12, 50-24, 79-24]]]), warray.value([[73, 7], [27, 1] ])],
			[warray.value([[1, [74-24]]]), warray.value([[50, 0], [50, 7]])],
			[warray.value([[1, [69-24]]]), warray.value([[100, -1]])],
		]],

		[1, [
			[warray.value([[1, [72-36, 79-48, 86-36, 60-24]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [72-48, 79-48, 86-48, 60]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [72-36, 79-24, 86-24, 60]]]), warray.value([[20, -1], [80, -1]])],

		]],
	])],

	[\kalimba, -30, warray.value([
		[1, [
			[warray.value([[1, [84]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [91]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [93]]]), warray.value([[20, 4], [80, 5]])],
			[warray.value([[1, [48]]]), warray.value([[50, 6], [50, 6]])],
			[warray.value([[1, [48, 79, 86, 60]]]), warray.value([[50, 6], [50, 7]])],
			[warray.value([[1, [72, 50, 79]]]), warray.value([[73, 7], [27, 1] ])],
			[warray.value([[1, [74]]]), warray.value([[50, 0], [50, 7]])],
			[warray.value([[1, [69]]]), warray.value([[100, -1]])],
		]],
		[1, [
			[warray.value([[1, [60]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [67]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [69]]]), warray.value([[20, 4], [80, 5]])],
			[warray.value([[1, [62]]]), warray.value([[50, 6], [50, 6]])],
			[warray.value([[1, [72, 79, 86, 60]]]), warray.value([[50, 6], [50, 7]])],
			[warray.value([[1, [72, 50, 79]]]), warray.value([[73, 7], [27, 1] ])],
			[warray.value([[1, [74]]]), warray.value([[50, 0], [50, 7]])],
			[warray.value([[1, [69]]]), warray.value([[100, -1]])],
		]],
	])],

	[\violin, -25, warray.value([
		[1, [
			[warray.value([[1, [84-48]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [91-48]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [93-48]]]), warray.value([[20, 4], [80, 5]])],
			[warray.value([[1, [48-12]]]), warray.value([[50, 6], [50, 6]])],
			[warray.value([[1, [48, 79-36, 86-48, 60]]]), warray.value([[50, 6], [50, 7]])],
			[warray.value([[1, [72-12, 50-24, 79-24]]]), warray.value([[73, 7], [27, 1] ])],
			[warray.value([[1, [74-24]]]), warray.value([[50, 0], [50, 7]])],
			[warray.value([[1, [69]]]), warray.value([[100, -1]])],
		]],
		[1, [
			[warray.value([[1, [60-24]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [67-24]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [69-24]]]), warray.value([[20, 4], [80, 5]])],
			[warray.value([[1, [50]]]), warray.value([[50, 6], [50, 6]])],
			[warray.value([[1, [72-36, 79-36, 86-48, 60]]]), warray.value([[50, 6], [50, 7]])],
			[warray.value([[1, [72-24, 50, 79-36]]]), warray.value([[73, 7], [27, 1] ])],
			[warray.value([[1, [74-24]]]), warray.value([[50, 0], [50, 7]])],
			[warray.value([[1, [69-12]]]), warray.value([[100, -1]])],
		]],

		[1, [
			[warray.value([[1, [72-36, 79-48, 86-36, 60-24]]]), warray.value([[80, 1], [20, 2]])],
			[warray.value([[1, [72-48, 79-48, 86-48, 60]]]), warray.value([[80, 3], [20, 4]])],
			[warray.value([[1, [72-36, 79-24, 86-24, 60]]]), warray.value([[20, -1], [80, -1]])],

		]],


	])],
];


durs = warray.value([
	[1, 1],
	[5, 2],
]);

p = {
	|layerCount = 1|

	"Playing % layers.".format(layerCount).postln;

	layerCount.do {
		|item|

		var layer = nodes[item];
		var currentNodes = layer[2].next;
		var nodeIndex = 0;

		var notes;
		var freq;
		var dur;
		var node;

		var time = s.latency;

		while ({ (nodeIndex >= 0) && (nodeIndex < currentNodes.size) }, {
			node = currentNodes[nodeIndex];
			nodeIndex = node[1].next;

			notes = node[0].next;
			dur = durs.next / bps;

			s.makeBundle(time, {
				notes.do {
					|item|
					var synth;

					freq = item.midicps;

					synth = Synth(layer[0], [
						\dur, dur,
						\amp, layer[1].dbamp,
						\freq, freq ,
						\attack, 0.5,
						\sustain, 0.5,
						\vel, 1+40.rand,
					]);
				};
			});

			time = time + dur;
		});
	}
};

if (o != nil, {
	o.free;
});

o = OSCFunc({
	|msg, time, addr, recvPort|
	if (msg.size > 1, {
		var layerCount = msg[1];
		p.value(layerCount);
	});
}, '/bounce');
)

(
Synth(\kalimba, [
	\dur, 1,
	\amp, -3.dbamp,
	\freq, 400 ,
	\attack, 0.5,
	\sustain, 0.5,
]);
)

(
if (q != nil, {
	q.stop;
});

q = Task({
	inf.do {
		p.value(5);
		7.wait;
	}
}).start;
)
