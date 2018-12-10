using CornerStoneApril30.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CornerStoneApril30.Reports
{
    public class TripReport
    {
        //Class constructor to pass the DateTime in
        public DateTime? ReportDate { get; set; }
        public TripReport(DateTime? rDate)
        {
            this.ReportDate = rDate;
        }
        #region
        int _totalColumn = 6;
        Document _document;
        Font _fontstyle;
        PdfPTable _pdfTable = new PdfPTable(6);
        PdfPCell _pdfPCell;
        MemoryStream _memoryStream = new MemoryStream();
        List<Trip> _trips = new List<Trip>();
        #endregion

        public byte[] PrepareReport(List<Trip> trips)
        {
            _trips = trips;

            #region
            _document = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            _document.SetPageSize(PageSize.A4);
            _document.SetMargins(20f, 20f, 20f, 20f);
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            _fontstyle = FontFactory.GetFont("Tahoma", 8f, 1);
            PdfWriter.GetInstance(_document, _memoryStream);
            _document.Open();
            _pdfTable.SetWidths(new float[] { 15f, 30f, 50f, 60f, 30f, 30f });
            #endregion

            this.ReportHeader();
            this.ReportBody();
            _pdfTable.HeaderRows = 3;
            _document.Add(_pdfTable);
            _document.Close();
            return _memoryStream.ToArray();
        }
        //Note need to pass a date in here for Title
        private void ReportHeader()
        {
            _fontstyle = FontFactory.GetFont("Tahoma", 11f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Trips - " + ReportDate.Value.DayOfWeek + " - " + ReportDate.Value.Date.ToShortDateString(), _fontstyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);
            _pdfTable.CompleteRow();

            _fontstyle = FontFactory.GetFont("Tahoma", 9f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Students", _fontstyle));
            _pdfPCell.Colspan = _totalColumn;
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.Border = 0;
            _pdfPCell.BackgroundColor = BaseColor.WHITE;
            _pdfPCell.ExtraParagraphSpace = 0;
            _pdfTable.AddCell(_pdfPCell);
            _pdfTable.CompleteRow();

            #region Table Header
            _fontstyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Bus", _fontstyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);
            _fontstyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Driver", _fontstyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);
            _fontstyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _pdfPCell = new PdfPCell(new Phrase("School", _fontstyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);
            _fontstyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Student", _fontstyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);
            _fontstyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Status", _fontstyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);
            _fontstyle = FontFactory.GetFont("Tahoma", 8f, 1);
            _pdfPCell = new PdfPCell(new Phrase("Logged Time", _fontstyle));
            _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            _pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            _pdfTable.AddCell(_pdfPCell);
            _pdfTable.CompleteRow();
            #endregion
        }

        private void ReportBody()
        {
            #region Table Body
            foreach (var t in _trips.Where(g => g.PickupTime == ReportDate))
            {
                var ts = t.TripAssignments.OrderBy(s => s.Student.School.Name).ThenBy(u => u.Student.LastName).ToList();
                _fontstyle = FontFactory.GetFont("Tahoma", 8f, 1);
                _pdfPCell = new PdfPCell();
                _pdfPCell.AddElement(new Paragraph(t.BusID.ToString(), _fontstyle) { Alignment = Element.ALIGN_CENTER });
                _pdfPCell.AddElement(new Paragraph("(" + t.TripAssignments.Count.ToString() + ")", _fontstyle) { Alignment = Element.ALIGN_CENTER });
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);
                _fontstyle = FontFactory.GetFont("Tahoma", 8f, 0);
                _pdfPCell = new PdfPCell(new Phrase(t.Driver.User.Fullname, _fontstyle));
                _pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
                _pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);
                _fontstyle = FontFactory.GetFont("Tahoma", 8f, 0);
                _pdfPCell = new PdfPCell();
                _pdfPCell.PaddingBottom = 10;
                _pdfPCell.PaddingLeft = 10;
                foreach (var tss in ts)
                {
                    _pdfPCell.AddElement(new Paragraph(tss.Student.School.Name, _fontstyle) { Alignment = Element.ALIGN_LEFT });
                }
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);
                _fontstyle = FontFactory.GetFont("Tahoma", 8f, 0);
                _pdfPCell = new PdfPCell();
                _pdfPCell.PaddingBottom = 10;
                _pdfPCell.PaddingLeft = 10;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                foreach (var tss in ts)
                {
                    _pdfPCell.AddElement(new Paragraph(tss.Student.Fullname, _fontstyle) { Alignment = Element.ALIGN_LEFT });
                }
                _pdfTable.AddCell(_pdfPCell);
                _fontstyle = FontFactory.GetFont("Tahoma", 8f, 0);
                _pdfPCell = new PdfPCell();
                _pdfPCell.PaddingBottom = 10;
                _pdfPCell.PaddingLeft = 10;
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                foreach (var tss in ts)
                {
                    _pdfPCell.AddElement(new Paragraph(tss.StatusOfPickUp.ToString(), _fontstyle) { Alignment = Element.ALIGN_LEFT });
                }
                _pdfTable.AddCell(_pdfPCell);

                _fontstyle = FontFactory.GetFont("Tahoma", 8f, 0);
                _pdfPCell = new PdfPCell();
                _pdfPCell.PaddingBottom = 10;
                foreach (var tss in ts)
                {
                    _pdfPCell.AddElement(new Paragraph(tss.LogTime, _fontstyle) { Alignment = Element.ALIGN_CENTER });
                }
                _pdfPCell.BackgroundColor = BaseColor.WHITE;
                _pdfTable.AddCell(_pdfPCell);
            }
            #endregion
        }
    }
}
