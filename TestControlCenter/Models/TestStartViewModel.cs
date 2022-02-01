using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TestControlCenterDomain;

namespace TestControlCenter.Models
{
    public class TestStartViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private List<Student> students;
        public List<Student> Students
        {
            get
            {
                if (string.IsNullOrEmpty(filter))
                {
                    return students;
                }
                else
                {
                    return students.Where(x => x.Name.Contains(filter) || x.NationalCode.Contains(filter) || x.Mobile.Contains(filter)).ToList();
                }
            }
            set => students = value;
        }
        public TestItemViewModel TestItem { get; set; }

        private string filter;
        public string Filter
        {
            get => filter;
            set
            {
                filter = value;
                OnPropertyChanged(nameof(Students));
            }
        }

        public bool StudentNotSelected => SelectedStudent == null;

        private Student selectedStudent;

        public Student SelectedStudent
        {
            get => selectedStudent;
            set
            {
                selectedStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
                OnPropertyChanged(nameof(StudentNotSelected));
            }
        }

        private bool secondPhase;
        public bool SecondPhase
        {
            get => secondPhase;
            set
            {
                secondPhase = value;
                OnPropertyChanged(nameof(SecondPhase));
            }
        }
    }
}
