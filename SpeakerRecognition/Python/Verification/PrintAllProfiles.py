import VerificationServiceHttpClientHelper
import sys

def print_all_profiles(subscription_key):
    """Print all the profiles for the given subscription key.

    Arguments:
    subscription_key -- the subscription key string
    """
    helper = VerificationServiceHttpClientHelper.VerificationServiceHttpClientHelper(
        subscription_key)

    profiles = helper.get_all_profiles()

    print('Profile ID, Locale, Enrollments Count, Remaining Enrollments Count,'
          ' Created Date Time, Last Action Date Time, Enrollment Status')
    for profile in profiles:
        print('{0}, {1}, {2}, {3}, {4}, {5}, {6}'.format(
            profile.get_profile_id(),
            profile.get_locale(),
            profile.get_enrollments_count(),
            profile.get_remaining_enrollments_count(),
            profile.get_created_date_time(),
            profile.get_last_action_date_time(),
            profile.get_enrollment_status()))

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print('Usage: python PrintAllProfiles.py <subscription_key>')
        print('\t<subscription_key> is the subscription key for the service')
        sys.exit('Error: Incorrect Usage.')

    print_all_profiles(sys.argv[1])
