using System;
using SFB;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using ZimGui;
namespace VerletBallSimulation {
    public class BallSimulation : MonoBehaviour {
        public PhysicsSolver Solver;
        public int Width = 10;
        public int Height = 10;
        public Vector2 Gravity = new Vector2(0, -9.8f);
        [Range(0,8)]
        public int StepsPerFrame = 8;

        BallRenderer _ballRenderer;
        [SerializeField]Material _material;
        [SerializeField]Material _blurMaterial;
        BallMaterial _ballMaterial;
        public int Count;
        public int TargetCount;
        int _steps;
        [SerializeField] Texture2D _texture;
        [Range(0.8f,1.2f)]
        public float CapacityFactor = 1;

        public void Start() {
            Application.targetFrameRate= 60;
            var capacity =(int) (Width * Height*CapacityFactor);
            Solver = new PhysicsSolver(Width,Height,capacity);
            Solver.Gravity = Gravity;
            _ballRenderer=new BallRenderer( capacity);
            _ballMaterial = new BallMaterial(_material,_blurMaterial);
            _wObject=IM.Add("Setting : Toggle with Esc",DrawSettings);
            _wObject.Opened = true;
            _wObject.Rect = new Rect(Screen.width - 400, 0 , 400,  Screen.height);
            IMStyle.FloatFormat = null;
            IMStyle.FontSize = 15;
        }
        WObject _wObject;
        [Range(0,50)]
        public int AddBallCount;
        [Range(4,100)]
        public int BallInterval;
        
        public Vector2 StartVelocity= new Vector2(0.5f,-0.2f);

        bool _instructionsFO;
        bool _projectileSettingFO;
        public bool UseDarkStar;
        public bool UsingDarkStar;
        public bool RetainDarkStar;
        public bool HideDarkStar;
        bool DrawSettings() {
            if (IM.Foldout("Instructions", ref _instructionsFO)) {
                using (IM.Indent()) {
                    IM.Label("RightMouse : Move Camera");
                    IM.Label("Mouse Scroll : Zoom Camera");
                    IM.Label("Dark Star : Gravity source");
                    IM.Label("LeftMouse : Move the darkStar");
                }
               
            }
            IM.Slider("StepsPerFrame",ref StepsPerFrame,0,8);
            IMStyle.FloatFormat = "F2";
            IM.Label("Simulation Time [ms]  ",_simTime);
            IMStyle.FloatFormat = null;
            IM.Label("Object Count : ",Solver.ObjectCount);
            IMStyle.DragNumberScale = Width * Height / 20f;
            IM.Slider("TargetCount",ref TargetCount,0,Solver.Capacity);
            using (IM.HorizontalDivide(3)) {
                if (IM.Button("Set Zero")) {
                    TargetCount=0;
                }if (IM.Button("Set Half")) {
                    TargetCount=Solver.Capacity/2;
                }
                if (IM.Button("Set Max")) {
                    TargetCount=Solver.Capacity;
                }
            }
            IMStyle.DragNumberScale = 1f;
            if(IM.Foldout("Projectile Setting",ref _projectileSettingFO)) {
                IMStyle.DragNumberScale = 0.1f;
                var ballScale = _ballMaterial.BallScale;
                if (IM.FloatField("Ball Draw Size", ref ballScale, 0.01f,5f)) {
                    _ballMaterial.BallScale= ballScale;
                }
                _ballMaterial.Blur=  IM.BoolField("Blur", _ballMaterial.Blur);
                IMStyle.DragNumberScale = 1f;
                IM.Vector2Field("Gravity",ref Gravity);
                IM.BoolField("UseDarkStar",ref UseDarkStar);
                if (UseDarkStar) {
                    IM.FloatField("DarkStarMass",ref DarkStar.Mass,-100,100);
                    IM.BoolField("RetainDarkStar",ref RetainDarkStar);
                    IM.BoolField("HideDarkStar",ref HideDarkStar);
                    
                }
                IM.Vector2Field("StartVelocity",ref StartVelocity);
                IM.Slider("AddBallCount",ref AddBallCount,0,50);
                IM.IntField("BallInterval",ref BallInterval,4,100);
            }
            
            if (InTextureState&&_texture != null)  IM.BoolField("UseTexture",ref InTextureState); 
            if(IM.Button("Open File Dialog")) {
                var paths = StandaloneFileBrowser.OpenFilePanel("Image", "", new [] {  new ExtensionFilter("Image Files", "jpg", "png")}, false);
                if (paths.Length > 0) {
                    var filePath = paths[0];
                    if (System.IO.File.Exists(filePath))
                    {
                        var fileData = System.IO.File.ReadAllBytes(filePath);
                        var tex = new Texture2D(2, 2);
                        tex.LoadImage(fileData); 
                        _texture = tex;
                    }
                }
            }
            
            if (_texture != null) {
                if (IM.Button("ApplyImage")) {
                    Apply();
                    InTextureState = true;
                    Solver.ObjectCount = 0;
                    _steps = 0;
                }
                IM.DrawTexture(200,_texture);
            }
            else {
                InTextureState = false;
            }
            return true;
        }
        bool _drag;

        public DarkStar DarkStar=new DarkStar(new float2(100,100),1000,1);
        public void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                _wObject.Opened =! _wObject.Opened;
            }

           
            if(!UseDarkStar||!RetainDarkStar) UsingDarkStar = false;
            if (IMInput.TargetID!=_wObject.WindowID) {
              
                if (UseDarkStar&&Input.GetMouseButton(0)) {
                    UsingDarkStar = true;
                    var pos= (Vector2) _ballMaterial.Position;
                    var scale= _ballMaterial.Scale;
                    var mousePos = ((Vector2) Input.mousePosition - pos)/scale;
                    DarkStar.Position = mousePos;
                }
                var scroll = Input.mouseScrollDelta.y;
                if (scroll != 0) {
                    var pos= (Vector2) _ballMaterial.Position;
                    var scale=_ballMaterial.Scale;
                    var  newScale = math.clamp(scale + scale*scroll * 0.1f, 1, 50);
                    var mousePos = (Vector2) Input.mousePosition-pos;
                    Vector2 delta = mousePos * (newScale/scale - 1f );
                    pos.x -= delta.x;
                    pos.y -= delta.y;
                    _ballMaterial.Position=pos;
                    _ballMaterial.Scale=newScale;
                }

            
                if (Input.GetMouseButtonDown(1)) {
                    _drag = true;
                }else if (Input.GetMouseButtonUp(1)) {
                    _drag = false;
                }else if (_drag&&Input.GetMouseButton(1)) {
                    var delta = IM.ScreenDeltaMousePos;
                    if (delta != default) {
                        var v =  _ballMaterial.Position;
                        v += (float2)delta;
                        _ballMaterial.Position = v;
                    }
                }
            }
            if (UsingDarkStar&&!HideDarkStar) {
                var scale= _ballMaterial.Scale;
                var pos= DarkStar.Position*scale+ _ballMaterial.Position;
                IM.Circle(pos, scale*DarkStar.Radius,new UiColor(255,255,255,127), scale*DarkStar.Radius*0.9f,new UiColor(0,0,0,127));

            }
            Solver.Gravity = Gravity;
            TargetCount=math.clamp(TargetCount,0,Solver.Capacity);
            if(TargetCount<Solver.ObjectCount) {
                Solver.ObjectCount=TargetCount;
                _steps = 0;
            }
            var count=Solver.ObjectCount;

            
            if(StepsPerFrame!=0) {
                StartVelocity.x=math.clamp(StartVelocity.x,64f/BallInterval,100);
                var s = ValueStopwatch.StartNew();
                for (int i = 0; i < StepsPerFrame; i++) {
                    if(AddBallCount!=0&&_steps%BallInterval==0) {
                        if (Solver.ObjectCount < TargetCount) {
                            float2 speed = new float2(StartVelocity.x/60f,StartVelocity.y/60f);
                            if (InTextureState) {
                                for (int j = 0; j < AddBallCount&&Solver.ObjectCount <TargetCount; j++) {
                                    Solver.AddObjectLastColor(new float2(2f, Height - 2 - j), speed);
                                }
                            }
                            else {
                                Color32 color = Color.HSVToRGB(count / (200f*AddBallCount) % 1f, 1, 1);
                                for (int j = 0; j < AddBallCount&&Solver.ObjectCount <TargetCount; j++) {
                                    Solver.AddObject(new float2(2f, Height - 2 - j), speed, color);
                                }
                            }
                           
                        }
                    }
                    _steps++;
                    if(UsingDarkStar)
                        Solver.Update(DarkStar,1f/ 480f);
                    else 
                        Solver.Update(1f/ 480f);
                }
                _simTime = s.GetElapsedTime().Ticks/10000f;
                _ballRenderer.Update(_ballMaterial.Current,Solver);
            }
            else {
                _simTime = 0;
            }
            _ballRenderer.Draw(_ballMaterial.Current);
            Count = Solver.ObjectCount;
        }

        float _simTime;
        public bool InTextureState;
       

        unsafe void Apply() {
            if (_texture == null) {
                return;
            }

            if (!_texture.isReadable) {
                _texture = DuplicateTexture(_texture);
            }
            if (_texture.graphicsFormat == GraphicsFormat.R8G8B8A8_UNorm) {
                var p = (Color32*) _texture.GetRawTextureData<Color32>().GetUnsafePtr();
                new ConvertColorArrayJob() {
                    WriteData = Solver.ColorArray.GetSubArray(0, Solver.ObjectCount),
                    TextureData = p,
                    PositionData = Solver.PositionArray.GetSubArray(0, Solver.ObjectCount),
                    TextureHeight = _texture.height,
                    TextureWidth = _texture.width,
                    GridHeight = Solver.Grid.Width,
                    GridWidth = Solver.Grid.Height,
                }.Schedule(Solver.ObjectCount, 64).Complete();
            }
            else {
                fixed ( Color32* p  = _texture.GetPixels32()) {
                    new ConvertColorArrayJob() {
                        WriteData = Solver.ColorArray.GetSubArray(0, Solver.ObjectCount),
                        TextureData = p,
                        PositionData = Solver.PositionArray.GetSubArray(0, Solver.ObjectCount),
                        TextureHeight = _texture.height,
                        TextureWidth = _texture.width,
                        GridHeight = Solver.Grid.Width,
                        GridWidth = Solver.Grid.Height,
                    }.Schedule(Solver.ObjectCount, 64).Complete();
                }
            }

            
        }
        static Texture2D DuplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);
 
            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
        void OnDestroy() {
            Solver.Dispose();
           _ballRenderer.Dispose();
        }
    }
}