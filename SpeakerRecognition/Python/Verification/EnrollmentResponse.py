class EnrollmentResponse:
    """This class encapsulates the enrollment response."""

    _ENROLLMENT_STATUS = 'enrollmentStatus'
    _ENROLLMENTS_COUNT = 'enrollmentsCount'
    _REMAINING_ENROLLMENTS = 'remainingEnrollments'
    _ENROLLMENT_PHRASE = 'phrase'

    def __init__(self, response):
        """Constructor of the EnrollmentResponse class.

        Arguments:
        response -- the dictionary of the deserialized python response
        """
        self._enrollment_status = response.get(self._ENROLLMENT_STATUS, None)
        self._enrollments_count = response.get(self._ENROLLMENTS_COUNT, None)
        self._remaining_enrollments = response.get(self._REMAINING_ENROLLMENTS, None)
        self._enrollment_phrase = response.get(self._ENROLLMENT_PHRASE, None)

    def get_enrollment_status(self):
        """Returns the enrollment status"""
        return self._enrollment_status

    def get_enrollments_count(self):
        """Returns the number of enrollments already performed"""
        return self._enrollments_count

    def get_enrollment_phrase(self):
        """Returns the enrollment phrase extracted from this request"""
        return self._enrollment_phrase

    def get_remaining_enrollments(self):
        """Returns the number of remaining enrollments before the profile is ready for verification"""
        return self._remaining_enrollments
