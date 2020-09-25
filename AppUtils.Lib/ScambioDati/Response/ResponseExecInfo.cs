using System;


namespace AppUtils.Lib.Response
{
    /// <summary>
    /// Identificatore univoco di esecuzione con timings di esecuzione
    /// </summary>
    public class ResponseExecInfo
    {
        public string Id { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public ResponseExecInfo(){
            this.Id = Guid.NewGuid().ToString();
            this.DateStart = DateTime.Now;
        }


        public void SetEnd() {
            this.DateEnd= DateTime.Now;
        }


    }

    


}
