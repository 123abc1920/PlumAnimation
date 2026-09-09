using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    /// <summary>
    /// Bone data
    /// </summary>
    public class Bone : ReactiveObject, IRenamable
    {
        public int id = 0;

        [Reactive]
        public string Name { get; set; } = "";
        public virtual bool IsBone
        {
            get { return true; }
        }

        [Reactive]
        public virtual double BaseX { get; set; }

        [Reactive]
        public virtual double BaseY { get; set; }

        [Reactive]
        public virtual double BaseA { get; set; }

        [Reactive]
        public virtual double AnimX { get; set; }

        [Reactive]
        public virtual double AnimY { get; set; }

        private double _animA;
        public virtual double AnimA
        {
            get => _animA;
            set
            {
                if (Math.Abs(_animA - value) > double.Epsilon)
                {
                    Rotate(value);
                    this.RaiseAndSetIfChanged(ref _animA, value);
                }
            }
        }

        public virtual double X
        {
            get
            {
                if (this._globalState == null)
                    return 0;
                return this._globalState.setBasePos ? this.BaseX : this.BaseX + this.AnimX;
            }
            set
            {
                if (this._globalState.setBasePos)
                {
                    this.BaseX = value;
                }
                else
                {
                    this.AnimX = value - this.BaseX;
                }
            }
        }

        public virtual double Y
        {
            get
            {
                if (this._globalState == null)
                    return 0;
                return this._globalState.setBasePos ? this.BaseY : this.BaseY + this.AnimY;
            }
            set
            {
                if (this._globalState.setBasePos)
                {
                    this.BaseY = value;
                }
                else
                {
                    this.AnimY = value - this.BaseY;
                }
            }
        }

        public virtual double A
        {
            get
            {
                if (this._globalState == null)
                    return 0;
                return this._globalState.setBasePos
                    ? this.BaseA
                    : (this.AnimA == 0 ? this.BaseA : this.AnimA);
            }
            set
            {
                if (this._globalState.setBasePos)
                {
                    this.BaseA = value;
                }
                else
                {
                    this.AnimA = value;
                }
            }
        }

        private double _shearX = 0;
        public double ShearX
        {
            get => _shearX;
            set
            {
                if (Math.Abs(_shearX - value) > double.Epsilon)
                {
                    _shearX = value;
                    this.RaiseAndSetIfChanged(ref _shearX, value);
                }
            }
        }

        private double _shearY = 0;
        public double ShearY
        {
            get => _shearY;
            set
            {
                if (Math.Abs(_shearY - value) > double.Epsilon)
                {
                    _shearY = value;
                    this.RaiseAndSetIfChanged(ref _shearY, value);
                }
            }
        }

        public double GlobalX
        {
            get
            {
                double localX = this.BaseX + (this._globalState.setBasePos ? 0 : this.AnimX);
                double localY = this.BaseY + (this._globalState.setBasePos ? 0 : this.AnimY);

                if (this.Parent != null)
                {
                    // Умножаем локальный вектор (localX, localY) на мировую матрицу родителя:
                    // X_global = Parent.GlobalX + (localX * Parent.G11 + localY * Parent.G21)
                    return this.Parent.GlobalX
                        + (localX * this.Parent.G11 + localY * this.Parent.G21);
                }

                return localX;
            }
            private set { }
        }

        public double GlobalY
        {
            get
            {
                double localY = this.BaseY + (this._globalState.setBasePos ? 0 : this.AnimY);
                double localX = this.BaseX + (this._globalState.setBasePos ? 0 : this.AnimX);

                if (this.Parent != null)
                {
                    // Y_global = Parent.GlobalY + (localX * Parent.G12 + localY * Parent.G22)
                    return this.Parent.GlobalY
                        + (localX * this.Parent.G12 + localY * this.Parent.G22);
                }

                return localY;
            }
            private set { }
        }

        public virtual double GlobalA
        {
            get
            {
                double angle = this.A;
                Bone? current = this.Parent;

                while (current != null)
                {
                    angle += current.A;
                    current = current.Parent;
                }

                return angle;
            }
        }

        private ObservableCollection<Slot> _slots = new ObservableCollection<Slot>();
        public ObservableCollection<Slot> Slots => _slots;

        public void UpdateSlots()
        {
            _slots.Clear();

            var newSlots = this._globalState.CurrentProject?.CurrentSkin?.GetSlots(this);
            if (newSlots != null)
            {
                foreach (var slot in newSlots)
                {
                    _slots.Add(slot);
                }
            }
        }

        public ObservableCollection<Bone> Children { get; set; } = new ObservableCollection<Bone>();
        public Bone? Parent { get; set; } = null;

        /// <summary>
        /// Returns name of the IRenamable object
        /// </summary>
        public string GetName
        {
            get => this.Name;
            set
            {
                if (this.Name != value)
                {
                    this.Name = value;
                }
            }
        }

        private double _lengthX = 10;
        public virtual double LengthX
        {
            get => _lengthX;
            set
            {
                if (_lengthX != value && value > 0)
                {
                    this.RaiseAndSetIfChanged(ref _lengthX, value);
                }
            }
        }
        public virtual double LengthY { get; set; } = 0;

        public double G11 { get; private set; } = 1;
        public double G12 { get; private set; } = 0;
        public double G21 { get; private set; } = 0;
        public double G22 { get; private set; } = 1;

        [Reactive]
        public SolidColorBrush BoneColor { get; set; } = new SolidColorBrush(Colors.Black);

        protected GlobalState _globalState;
        protected LocalizationService _localizationService;

        protected Bone()
        {
            this.WhenAnyValue(x => x.BaseX, x => x.AnimX)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(X)));

            this.WhenAnyValue(x => x.BaseY, x => x.AnimY)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(Y)));

            this.WhenAnyValue(x => x.BaseA, x => x.AnimA)
                .Subscribe(_ => this.RaisePropertyChanged(nameof(A)));

            this.BoneColor = this.GenerateRandomColor();
        }

        public Bone(GlobalState globalState, LocalizationService localizationService)
            : this()
        {
            this.Name = "root";

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        public Bone(
            GlobalState globalState,
            Bone parent,
            int _id,
            LocalizationService localizationService
        )
            : this()
        {
            this.id = _id;
            string name = "bone";
            this.Name = $"{name}{Counter.GenerateNamePostfix()}";

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        public Bone(GlobalState globalState, Bone parent, LocalizationService localizationService)
            : this()
        {
            string name = "bone";
            this.Name = $"{name}{Counter.GenerateNamePostfix()}";

            this.Parent = parent;

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        public Bone(GlobalState globalState, string name, LocalizationService localizationService)
            : this()
        {
            this.Name = name;

            this.id = 100;

            this._globalState = globalState;
            this._localizationService = localizationService;
        }

        public void AddChildren(Bone bone)
        {
            this.Children.Add(bone);
            bone.Parent = this;
        }

        private bool _isMoving = false;

        /// <summary>
        /// Moves bone and all its children and slots to new position
        /// </summary>
        /// <param name="x">Target x coordinate (global)</param>
        /// <param name="y">Target y coordinate (global)</param>
        public virtual void Move(double x, double y)
        {
            if (_isMoving)
                return;
            _isMoving = true;

            if (this.Parent != null)
            {
                double dx = x - this.Parent.GlobalX;
                double dy = y - this.Parent.GlobalY;

                double parentAngleRad = -this.Parent.GlobalA * Math.PI / 180;
                double localX = dx * Math.Cos(parentAngleRad) - dy * Math.Sin(parentAngleRad);
                double localY = dx * Math.Sin(parentAngleRad) + dy * Math.Cos(parentAngleRad);

                if (this._globalState.setBasePos)
                {
                    this.BaseX = localX;
                    this.BaseY = localY;
                }
                else
                {
                    this.AnimX = localX - this.BaseX;
                    this.AnimY = localY - this.BaseY;
                }

                this.X = localX;
                this.Y = localY;
            }
            else
            {
                if (this._globalState.setBasePos)
                {
                    this.BaseX = x;
                    this.BaseY = y;
                }
                else
                {
                    this.AnimX = x - this.BaseX;
                    this.AnimY = y - this.BaseY;
                }

                this.X = x;
                this.Y = y;
            }

            _isMoving = false;
        }

        private bool _isRotating = false;

        /// <summary>
        /// Rotates bone to new angle
        /// </summary>
        /// <param name="a">Target angle</param>
        public virtual void Rotate(double a)
        {
            if (_isRotating)
                return;

            _isRotating = true;

            this.A = a;

            _isRotating = false;
        }

        /// <summary>
        /// Changes bone`s length
        /// </summary>
        /// <param name="x">Click x coordinate</param>
        /// <param name="y">Click y coordinate</param>
        public virtual void Scale(double x, double y)
        {
            this.LengthX = Math.Sqrt((x - this.X) * (x - this.X) + (y - this.Y) * (y - this.Y));
        }

        public double ScaleX { get; set; } = 1;
        public double ScaleY { get; set; } = 1;

        public virtual void Shear(double x, double y)
        {
            this.ShearX = x;
            this.ShearY = y;
        }

        /// <summary>
        /// Draws bone with matrix logic matching Spine 2D
        /// </summary>
        public void DrawBone(
            Canvas canvas,
            double m11 = 1,
            double m12 = 0,
            double m21 = 0,
            double m22 = 1,
            double parentX = 0,
            double parentY = 0
        )
        {
            // 1. Переводим углы в радианы
            double rotationRad = this.A * Math.PI / 180;
            double shearXRad = this.ShearX * Math.PI / 180;
            double shearYRad = this.ShearY * Math.PI / 180;

            // 2. В Spine оси X и Y независимы:
            // Ось X повернута на (rotation + shearX)
            // Ось Y повернута на (rotation + 90 + shearY)
            double angleX = rotationRad + shearXRad;
            double angleY = rotationRad + Math.PI / 2 + shearYRad;

            // 3. Направления осей (единичные векторы)
            double cosX = Math.Cos(angleX) * this.ScaleX;
            double sinX = Math.Sin(angleX) * this.ScaleX;
            double cosY = Math.Cos(angleY) * this.ScaleY;
            double sinY = Math.Sin(angleY) * this.ScaleY;

            // 4. Локальная матрица
            double local11 = cosX;
            double local12 = sinX;
            double local21 = cosY;
            double local22 = sinY;

            // 5. Умножаем на родительскую матрицу
            double g11 = m11 * local11 + m21 * local12;
            double g12 = m12 * local11 + m22 * local12;
            double g21 = m11 * local21 + m21 * local22;
            double g22 = m12 * local21 + m22 * local22;

            // 6. Мировые координаты
            double globalX = parentX + (this.X * m11 + this.Y * m21);
            double globalY = parentY + (this.X * m12 + this.Y * m22);

            // 7. Конец кости
            double endX = globalX + (this.LengthX * g11);
            double endY = globalY + (this.LengthX * g12);

            // Сохраняем матрицу
            this.G11 = g11;
            this.G12 = g12;
            this.G21 = g21;
            this.G22 = g22;
            this.GlobalX = globalX;
            this.GlobalY = globalY;

            // Отрисовка
            Point start = new Point(canvas.Width / 2 + globalX, canvas.Height / 2 + globalY);
            Point end = new Point(canvas.Width / 2 + endX, canvas.Height / 2 + endY);

            var line = new Line
            {
                StartPoint = start,
                EndPoint = end,
                Stroke = this._globalState.GetLineBoneColor(this),
                StrokeThickness = 3,
            };

            var joint = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = this._globalState.GetDotBoneColor(this),
            };

            Canvas.SetLeft(joint, start.X - 4);
            Canvas.SetTop(joint, start.Y - 4);

            canvas.Children.Add(line);
            canvas.Children.Add(joint);

            // Рекурсивно отрисовываем дочерние кости
            foreach (var childBone in this.Children)
            {
                childBone.DrawBone(canvas, g11, g12, g21, g22, globalX, globalY);
            }
        }

        private SolidColorBrush GenerateRandomColor()
        {
            var random = new Random();
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);
            return new SolidColorBrush(new Color(255, r, g, b));
        }

        /// <summary>
        /// Returns JSON data
        /// </summary>
        public BoneData GenerateJSONData()
        {
            return new BoneData
            {
                Name = this.Name,
                Parent = this.Parent?.Name,
                X = this.BaseX,
                Y = this.BaseY,
                Rotation = this.BaseA,
            };
        }

        /// <summary>
        /// Returns JSON string
        /// </summary>
        public string GenerateCode()
        {
            return JsonConvert.SerializeObject(GenerateJSONData(), this._globalState.jsonSettings);
        }

        /// <summary>
        /// Sets new name to IRenamable object
        /// </summary>
        /// <param name="name">New name</param>
        public void SetName(string? name)
        {
            if (this._globalState.CurrentProject.IsUniqBone(name) == true)
            {
                if (name != null)
                {
                    this.Name = name;
                }
            }
        }
    }

    /// <summary>
    /// Jsonifyed bone data
    /// </summary>
    public class BoneData
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public string? Parent { get; set; }

        [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double X { get; set; }

        [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double Y { get; set; }

        [JsonProperty("rotation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double Rotation { get; set; }
    }
}
